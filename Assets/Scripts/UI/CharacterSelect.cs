using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private GameObject listItemPrefab;
    
    private CharacterInfo _characterInfo;
    private CountdownTimer _saveTimer;
    private bool _loadInitiated = false;
    
    private void Update()
    {
        if (_saveTimer == null) return;
        
        _saveTimer.Tick(Time.deltaTime);
        if (_saveTimer.IsFinished && !_loadInitiated)
        {
            _loadInitiated = true;
            LoadNextScene();
        }
    }
    
    public void EmbarkWithCharacter(CharacterInfo characterInfo)
    {
        _characterInfo = characterInfo;
        
        GameManager.Instance.CharacterType = characterInfo.characterType;
        GameObject playerObject = GameManager.Instance.SpawnPlayer();
        GameManager.Instance.Player = playerObject.GetComponent<Player>();

        InventoryManager.Instance.OnStartDone += FinishEmbark;
    }

    public void FinishEmbark()
    {
        if (_characterInfo.startingConsumable1)
        {
            InventoryManager.Instance.AddItem(_characterInfo.startingConsumable1, 5);
            InventoryManager.Instance.AddConsumableListItem(Instantiate(listItemPrefab), _characterInfo.startingConsumable1);
        }
        if (_characterInfo.startingConsumable2)
        {
            InventoryManager.Instance.AddItem(_characterInfo.startingConsumable2, 5);
            InventoryManager.Instance.AddConsumableListItem(Instantiate(listItemPrefab), _characterInfo.startingConsumable2);
        }
        if (_characterInfo.startingSpell1)
        {
            InventoryManager.Instance.AddItem(_characterInfo.startingSpell1, 0);
            InventoryManager.Instance.AddSpellListItem(Instantiate(listItemPrefab), _characterInfo.startingSpell1);
        }
        if (_characterInfo.startingSpell2)
        {
            InventoryManager.Instance.AddItem(_characterInfo.startingSpell2, 0);
            InventoryManager.Instance.AddSpellListItem(Instantiate(listItemPrefab), _characterInfo.startingSpell2);
        }
        
        SaveSystem.Save();
        SceneManager.LoadScene(levelName);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(levelName);
    }
}
