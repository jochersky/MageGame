using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Info/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public string characterName;
    public string giftDescription;
    public BaseStats characterStats;
    public SpellConfig startingSpell1;
    public SpellConfig startingSpell2;
    public ConsumableConfig startingConsumable1;
    public ConsumableConfig startingConsumable2;
}
