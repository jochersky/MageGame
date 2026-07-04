using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private string levelName;
    
    public void EmbarkWithCharacter(CharacterInfo characterInfo)
    {
        Debug.Log("embarking with: " + characterInfo.characterName);

        GameManager.Instance.CharacterType = characterInfo.characterType;
        SaveSystem.Save();
        
        SceneManager.LoadScene(levelName);
    }
}
