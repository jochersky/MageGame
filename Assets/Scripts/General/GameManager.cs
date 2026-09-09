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
    [SerializeField] private GameObject listItemPrefab;
    
    [Header("Debugging")]
    [SerializeField] private bool debug;
    [SerializeField] private GameObject debugPlayerObject;

    private bool _playerComponentLoaded = false;
    private bool _inventoryManagerLoaded = false;

    private CountdownTimer _loadTimer;
    
    public Player Player { get; set; }
    public Health PlayerHealth { get; set; }
    public SpellManager SpellManager { get; set; }
    public InventoryUI InventoryUI => inventoryUI;
    public FullScreenEffect FullScreenEffect => fullScreenEffect;
    
    public static GameManager Instance { get; private set; }
    
    public CharacterType CharacterType { get; set; }

    public bool PlayerComponentLoaded { get => _playerComponentLoaded;
        set
        {
            _playerComponentLoaded = value;
            LoadPlayerStatsAndItems();
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
            Player playerComponent = debugPlayerObject.GetComponent<Player>();
            playerComponent.HealthBar = healthBar;
            playerComponent.ManaBar = manaBar;
            SaveSystem.Delete();
            return;
        }
        
        // for character chosen through character select
        if (debugPlayerObject) Destroy(debugPlayerObject);
        if (SaveSystem.SaveDataExists()) SaveSystem.Load();
        if (spawnPoint)
        {
            GameObject playerInst = SpawnPlayer();
            if (mapGenerator) mapGenerator.player = playerInst;
            
            Player playerComponent = playerInst.GetComponent<Player>();
            playerComponent.HealthBar = healthBar;
            playerComponent.ManaBar = manaBar;
            Player = playerComponent;
            
            // load player's items once these scripts have finished running
            playerComponent.OnStartDone += () => { PlayerComponentLoaded = true; };
            InventoryManager.Instance.OnStartDone += () => { InventoryManagerLoaded = true; };
        }
        
        equippedUI?.SubscribeToEvents();
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

    public void LoadPlayerStatsAndItems()
    {
        if (!_playerComponentLoaded || !_inventoryManagerLoaded) return;
        
        SaveSystem.Load();
    }

    public void Save(ref GameSaveData data)
    {
        data.characterType = CharacterType;
    }

    public void Load(ref SaveSystem.SaveData data)
    {
        CharacterType = data.gameSaveData.characterType;
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
}