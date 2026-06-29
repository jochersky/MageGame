using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableConsumableStrategy", menuName = "Consumable Strategies/PlaceableConsumableStrategy")]
public class PlaceableConsumableStrategy : ConsumableStrategy
{
    public override void UsePlaceableConsumable(Transform spawnTransform, Vector3 spawnPosition)
    {
        // using spawn transform lets consumable be flipped
        GameObject inst = Instantiate(prefab, spawnTransform);
        inst.transform.position = spawnPosition;
        // null so that it won't follow the player's movement 
        inst.transform.parent = null;
    }
}