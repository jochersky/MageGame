using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private Button pyromancerEmbarkButton;
    [SerializeField] private Button character2EmbarkButton;
    [SerializeField] private Button character3EmbarkButton;

    private void Start()
    {
        
    }

    public void EmbarkWithCharacter(CharacterInfo characterInfo)
    {
        Debug.Log("embarking with: " + characterInfo.characterName);
    }
}
