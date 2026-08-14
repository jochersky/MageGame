using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableConfig", menuName = "ScriptableObjects/ConsumableConfig")]
public class ConsumableConfig : ItemConfig
{
    [Header("Consumable Info")] 
    public int maxCount;
    public ConsumableType type;
    public bool snapToGrid;
    public int damage;
    public ConsumableStrategy strategy;
}

public enum ConsumableType
{
    Base,
    Placeable,
    Throwable
}
