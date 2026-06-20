using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "Stats/BaseStats")]
public class BaseStats : ScriptableObject
{
    public int health = 1;
    public int jumps = 1;
    public float speed = 1f;
}
