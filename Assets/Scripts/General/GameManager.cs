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
    [Header("UI References")]
    [SerializeField] private HUDBar healthBar;
    [SerializeField] private HUDBar manaBar;
    
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

    private void Update()
    {
        // if (Keyboard.current.numpad1Key.wasPressedThisFrame) SaveSystem.Save();
        // if (Keyboard.current.numpad2Key.wasPressedThisFrame) SaveSystem.Load();
    }

    public GameObject SpawnPlayer()
    {
        switch (CharacterType)
        {
            case CharacterType.Base: return Instantiate(baseCharacterPrefab, spawnPoint); break;
            case CharacterType.Pyromancer: return Instantiate(pyromancerPrefab, spawnPoint); break;
            case CharacterType.Hound: return Instantiate(houndPrefab, spawnPoint); break;
            case CharacterType.Warden: return Instantiate(wardenPrefab, spawnPoint); break;
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

    public void Load(ref GameSaveData data)
    {
        CharacterType = data.characterType;
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