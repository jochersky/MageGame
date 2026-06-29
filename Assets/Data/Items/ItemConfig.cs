using UnityEngine;

public enum ItemType
{
    Spell,
    Consumable
}

public abstract class ItemConfig : ScriptableObject
{
    [Header("Item Info")]
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public bool changePositionOnObstruction;
}
