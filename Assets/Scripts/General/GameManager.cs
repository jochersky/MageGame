using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject baseCharacterPrefab;
    [SerializeField] private GameObject pyromancerPrefab;
    [SerializeField] private GameObject houndPrefab;
    [SerializeField] private GameObject wardenPrefab;
    
    [Header("Debugging")]
    [SerializeField] private bool debug;
    [SerializeField] private GameObject debugPlayerObject;
    
    public Player Player { get; set; }
    public Health PlayerHealth { get; set; }
    public SpellManager SpellManager { get; set; }
    
    public static GameManager Instance { get; private set; }
    
    public CharacterType CharacterType { get; set; }
    
    private void Awake()
    {
        // Ensure only one instance of the inventory exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        if (debug) return;
        
        if (debugPlayerObject) Destroy(debugPlayerObject);
        SaveSystem.Load();
        if (spawnPoint)
        {
            switch (CharacterType)
            {
                case CharacterType.Base: Instantiate(baseCharacterPrefab, spawnPoint); break;
                case CharacterType.Pyromancer: Instantiate(pyromancerPrefab, spawnPoint); break;
                case CharacterType.Hound: Instantiate(houndPrefab, spawnPoint); break;
                case CharacterType.Warden: Instantiate(wardenPrefab, spawnPoint); break;
            }
        }
    }

    private void Update()
    {
        if (Keyboard.current.numpad1Key.wasPressedThisFrame) SaveSystem.Save();
        if (Keyboard.current.numpad2Key.wasPressedThisFrame) SaveSystem.Load();
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