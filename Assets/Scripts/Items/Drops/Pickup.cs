using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            PickUpEffect();
            Destroy(gameObject);
        }
    }


    public abstract void PickUpEffect();
}
