using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Barrel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGERED!");
        if (collision.TryGetComponent(out Hitbox hitbox))
        {
            Debug.Log("TRIGGERED HITBOX!");
            Destroy(gameObject);
        }
    }
}
