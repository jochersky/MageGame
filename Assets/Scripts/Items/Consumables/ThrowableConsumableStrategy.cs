using UnityEngine;

[CreateAssetMenu(fileName = "ThrowableConsumableStrategy", menuName = "Consumable Strategies/ThrowableConsumableStrategy")]
public class ThrowableConsumableStrategy : ConsumableStrategy
{
    public override void UseThrowingConsumable(Transform spawnTransform, Vector3 spawnPosition, Vector3 direction, Vector3 velocity)
    {
        // using spawn transform lets consumable be flipped
        GameObject inst = Instantiate(prefab, spawnTransform);
        inst.transform.position = spawnPosition;
        // null so that it won't follow the player's movement 
        inst.transform.parent = null;

        // "throw" in direction of player movement
        Rigidbody2D rb = inst.GetComponentInChildren<Rigidbody2D>();
        rb.linearVelocityX = direction.x * 15f;
        rb.linearVelocityY = velocity.y * 2f;
    }
}