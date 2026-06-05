using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableStrategy", menuName = "Consumable Strategies/ConsumableStrategy")]
public class ConsumableStrategy : ScriptableObject
{
    public GameObject prefab;

    public virtual void UseConsumable(Transform spawnTransform, Vector3 spawnPosition)
    {
        // using spawn transform lets consumable be flipped
        GameObject inst = Instantiate(prefab, spawnTransform);
        inst.transform.position = spawnPosition;
        // null so that it won't follow the player's movement 
        inst.transform.parent = null;
    }

    // TODO : bomb
    public virtual void UseThrowingConsumable(Transform spawnTransform, Vector3 spawnPosition, Vector3 direction, Vector3 velocity) { }
    // TODO : rope
    public virtual void UsePlaceableConsumable(Transform spawnTransform, Vector3 spawnPosition) { }
}