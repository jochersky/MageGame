using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
    [SerializeField] private CharacterInfo characterInfo;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI mana;
    [SerializeField] private TextMeshProUGUI gift;
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell2Image;
    [SerializeField] private Image consumable1Image;
    [SerializeField] private Image consumable2Image;
    
    private void Start()
    {
        if (!characterInfo) return;
        
        name.text = characterInfo.characterName;
        health.text = "Health: " + characterInfo.characterStats.health.ToString();
        mana.text = "Mana: " + characterInfo.characterStats.mana.ToString();
        gift.text = characterInfo.giftDescription;

        if (characterInfo.startingSpell1)
        {
            spell1Image.color = Color.white;
            spell1Image.sprite = characterInfo.startingSpell1.icon;
        }
        if (characterInfo.startingSpell2)
        {
            spell2Image.color = Color.white;
            spell2Image.sprite = characterInfo.startingSpell2.icon;
        }
        if (characterInfo.startingConsumable1)
        {
            consumable1Image.color = Color.white;
            consumable1Image.sprite = characterInfo.startingConsumable1.icon;
        }
        if (characterInfo.startingConsumable2)
        {
            consumable2Image.color = Color.white;
            consumable2Image.sprite = characterInfo.startingConsumable2.icon;
        }
    }
}
