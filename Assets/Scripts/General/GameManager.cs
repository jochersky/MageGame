using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private InventoryUI inventoryUI;
    
    [Header("Debugging")]
    [SerializeField] private bool debug;
    [SerializeField] private GameObject debugPlayerObject;
    
    private bool _playerComponentLoaded;
    private bool _inventoryManagerLoaded;

    private CountdownTimer _loadTimer;
    
    public Player Player { get; set; }
    public Health PlayerHealth { get; set; }
    public SpellManager SpellManager { get; set; }
    
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
        // Ensure only one instance of the inventory exists globally
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
            
            // load player's items once these scripts have finished running
            playerComponent.OnStartDone += () => { PlayerComponentLoaded = true; };
            InventoryManager.Instance.OnStartDone += () => { InventoryManagerLoaded = true; };
        }
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