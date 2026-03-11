using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Barrel : MonoBehaviour
{
    bool triggered = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Hitbox hitbox) && !triggered)
        {
            triggered = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInChildren<AudioSource>().Play();
        }
    }
}
