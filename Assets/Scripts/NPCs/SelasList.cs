using UnityEngine;

[CreateAssetMenu(fileName = "SelasList", menuName = "Scriptable Objects/SelasList")]
public class SelasList : ScriptableObject
{
    public ItemConfig[] purchaseableItems;
    public SpellConfig[] purchaseableSpells;
}
