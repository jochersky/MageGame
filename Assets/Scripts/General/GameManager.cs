using System;
using System.Data;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class GameManager : MonoBehaviour
{
    [Header("Level References")]
    [SerializeField] MapGenerator mapGenerator;
    
    [Header("Player References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject baseCharacterPrefab;
    [SerializeField] private GameObject pyromancerPrefab;
    [SerializeField] private GameObject houndPrefab;
    [SerializeField] private GameObject wardenPrefab;
    [SerializeField] private CharacterInfo baseCharacterInfo;
    [SerializeField] private CharacterInfo pyromancerInfo;
    [SerializeField] private CharacterInfo houndInfo;
    [SerializeField] private CharacterInfo wardenInfo;
    [Header("UI References")]
    [SerializeField] private HUDBar healthBar;
    [SerializeField] private HUDBar manaBar;
    [SerializeField] private FullScreenEffect fullScreenEffect;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private EquippedUI equippedUI;
    [SerializeField] private PostRunStatsMenuUI postRunStatsMenuUI;
    [SerializeField] private GameObject listItemPrefab;
    [Header("Debugging")]
    [SerializeField] private bool debug;
    [SerializeField] private GameObject debugPlayerObject;

    private MapGenerator _mapGenerator;
    
    private bool _playerComponentLoaded = false;
    private bool _inventoryManagerLoaded = false;
    
    private CountdownTimer _loadTimer;
    
    // Stats 
    private string _currentStage;
    private int _runSeed;
    private float _runTime;
    private int _enemiesSlain;
    private int _damageTaken;
    private int _moneyCollected;
    private int _wishesMade;
    
    // References
    public static GameManager Instance { get; private set; }
    public Player Player { get; set; }
    public Health PlayerHealth => Player.Health;
    public SpellManager SpellManager { get; set; }
    public InventoryUI InventoryUI => inventoryUI;
    public FullScreenEffect FullScreenEffect => fullScreenEffect;
    public PostRunStatsMenuUI PostRunStatsMenuUI => postRunStatsMenuUI;
    
    public CharacterType CharacterType { get; set; }
    public int EnemiesSlain { get => _enemiesSlain; set => _enemiesSlain = value; }

    public bool PlayerComponentLoaded { get => _playerComponentLoaded;
        set
        {
            _playerComponentLoaded = value;
            SubscribeToPlayerInputEvents();
            LoadPlayerStatsAndItems();
            ConnectStatUpdates();
        }
    }
    
    public bool InventoryManagerLoaded { get => _inventoryManagerLoaded;
        set
        {
            _inventoryManagerLoaded = value;
            LoadPlayerStatsAndItems();
        }
    }

    private void Awake()
    {
        // Ensure only one instance of the game manager exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        // for character already placed in scene
        if (debug)
        {
            AssignPlayerAndReferences(debugPlayerObject);
            SaveSystem.Delete();
            SubscribeToPlayerInputEvents();
            return;
        }
        
        // for character chosen through character select
        if (debugPlayerObject) Destroy(debugPlayerObject);
        if (SaveSystem.SaveDataExists()) SaveSystem.Load();
        if (spawnPoint)
        {
            GameObject playerInst = SpawnPlayer();
            if (mapGenerator) mapGenerator.player = playerInst;
            
            AssignPlayerAndReferences(playerInst);
            
            // load player's items once these scripts have finished running
            Player.OnStartDone += () => { PlayerComponentLoaded = true; };
            InventoryManager.Instance.OnStartDone += () => { InventoryManagerLoaded = true; };
        }
        
        equippedUI?.SubscribeToEvents();
        
        _mapGenerator = FindFirstObjectByType<MapGenerator>();
        
        _runTime = Time.time;
    }

    private void AssignPlayerAndReferences(GameObject playerGO)
    {
        Player = playerGO.GetComponent<Player>();
        Player.HealthBar = healthBar;
        Player.ManaBar = manaBar;
    }
    
    // handles event subscription for other scripts that need access to the player's PlayerInput input events 
    private void SubscribeToPlayerInputEvents()
    {
        InputManager input = Player.GetComponent<InputManager>();
        
        input.OnPlayerPressedInventory += inventoryUI.OnInventoryPressed;
        input.OnPlayerPressedCancel += inventoryUI.OnCancelPressed;
    }

    public GameObject SpawnPlayer()
    {
        switch (CharacterType)
        {
            case CharacterType.Base: 
                inventoryUI?.UpdateStatsScreen(baseCharacterInfo.characterStats.health, baseCharacterInfo.characterStats.mana, CharacterType.Base, baseCharacterInfo.giftDescription);
                return Instantiate(baseCharacterPrefab, spawnPoint);
            case CharacterType.Pyromancer: 
                inventoryUI?.UpdateStatsScreen(pyromancerInfo.characterStats.health, pyromancerInfo.characterStats.mana, CharacterType.Pyromancer, pyromancerInfo.giftDescription);
                return Instantiate(pyromancerPrefab, spawnPoint);
            case CharacterType.Hound: 
                inventoryUI?.UpdateStatsScreen(houndInfo.characterStats.health, houndInfo.characterStats.mana, CharacterType.Hound, houndInfo.giftDescription);
                return Instantiate(houndPrefab, spawnPoint);
            case CharacterType.Warden:
                inventoryUI?.UpdateStatsScreen(wardenInfo.characterStats.health, wardenInfo.characterStats.mana, CharacterType.Warden, wardenInfo.giftDescription);
                return Instantiate(wardenPrefab, spawnPoint);
        }

        return null;
    }

    public void ResetStatsAndItems()
    {
        ResetStats();
        
        // Remove items from inventory
        InventoryManager.Instance.ClearItemData();
        
        // Get character info to add starting items back to inventory
        CharacterInfo characterInfo = baseCharacterInfo;
        switch (CharacterType)
        {
            case CharacterType.Base: characterInfo = baseCharacterInfo; break;
            case CharacterType.Pyromancer: characterInfo = pyromancerInfo; break;
            case CharacterType.Hound: characterInfo = houndInfo; break;
            case CharacterType.Warden: characterInfo = wardenInfo; break;
        }
        
        if (characterInfo.startingConsumable1)
        {
            InventoryManager.Instance.AddItem(characterInfo.startingConsumable1, 5);
            InventoryManager.Instance.AddConsumableListItem(Instantiate(listItemPrefab), characterInfo.startingConsumable1);
        }
        if (characterInfo.startingConsumable2)
        {
            InventoryManager.Instance.AddItem(characterInfo.startingConsumable2, 5);
            InventoryManager.Instance.AddConsumableListItem(Instantiate(listItemPrefab), characterInfo.startingConsumable2);
        }
        if (characterInfo.startingSpell1)
        {
            // InventoryManager.Instance.AddItem(characterInfo.startingSpell1, 0);
            InventoryManager.Instance.AddSpellListItem(Instantiate(listItemPrefab), characterInfo.startingSpell1);
        }
        if (characterInfo.startingSpell2)
        {
            // InventoryManager.Instance.AddItem(characterInfo.startingSpell2, 0);
            InventoryManager.Instance.AddSpellListItem(Instantiate(listItemPrefab), characterInfo.startingSpell2);
        }
        
        // Reset health and mana values
        Player.UpdateHealthAndMana();
        
        // Reset money counter
        InventoryManager.Instance.UpdateMoney(-InventoryManager.Instance.GetMoneyCount());
    }

    private void ResetStats()
    {
        _enemiesSlain = 0;
        _damageTaken = 0;
        _moneyCollected = 0;
        _wishesMade = 0;
        _runSeed = 0;
        _runTime = 0;
    }

    public void LoadPlayerStatsAndItems()
    {
        if (!_playerComponentLoaded || !_inventoryManagerLoaded) return;
        
        SaveSystem.Load();
    }

    public void Save(ref GameSaveData data)
    {
        data.characterType = CharacterType;
        data.enemiesSlain = _enemiesSlain;
        data.damageTaken = _damageTaken;
        data.moneyCollected = _moneyCollected;
        data.wishesMade = _wishesMade;
        if (_mapGenerator)
        {
            data.currentStage = _mapGenerator.Level;
            data.runSeed = _mapGenerator.Seed;
        }
        data.runTime += Time.time - _runTime;
    }

    public void Load(ref SaveSystem.SaveData data)
    {
        CharacterType = data.gameSaveData.characterType;
        _enemiesSlain = data.gameSaveData.enemiesSlain;
        _damageTaken = data.gameSaveData.damageTaken;
        _moneyCollected = data.gameSaveData.moneyCollected;
        _wishesMade = data.gameSaveData.wishesMade;
        _runSeed = data.gameSaveData.runSeed;
        _currentStage = data.gameSaveData.currentStage;
        // load for runTime unnecessary since it gets overridden to capture current time
    }

    private void ConnectStatUpdates()
    {
        // Enemies slain updated through setter
        PlayerHealth.OnDamageTaken += properties => { _damageTaken += properties.amount; };
        InventoryManager.Instance.OnMoneyUpdated += updateAmt => { if (updateAmt > 0) _moneyCollected += updateAmt; };
        Player.OnWishMade += () => { _wishesMade++; };
    }
}

public enum CharacterType
{
    Base,
    Pyromancer,
    Hound,
    Warden
}

[System.Serializable]
public struct GameSaveData
{
    public CharacterType characterType;
    public string currentStage;
    public int runSeed;
    public float runTime;
    public int enemiesSlain;
    public int damageTaken;
    public int moneyCollected;
    public int wishesMade;
}