using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject enemy; 
    bool triggered = false;
    System.Random randy;
    void Start()
    {
        randy = new System.Random();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Hitbox hitbox) && !triggered)
        {
            triggered = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInChildren<AudioSource>().Play();


            // Spawn random item
            if (randy.Next(0, 100) < 25)
            {
                Instantiate(enemy, transform);
            }
        }
    }
}
