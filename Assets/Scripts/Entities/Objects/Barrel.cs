using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject enemy; 
    [SerializeField] GameObject manaCapsule;
    [SerializeField] GameObject coin;
    readonly List<GameObject> potentialDrops = new();
    bool triggered = false;
    System.Random randy;
    void Start()
    {
        randy = new System.Random();
        potentialDrops.Add(enemy);
        potentialDrops.Add(manaCapsule);
        potentialDrops.Add(coin);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Hitbox hitbox) && !triggered)
        {
            triggered = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInChildren<AudioSource>().Play();


            // Spawn random drop at 25% chance
            if (randy.Next(0, 100) < 75)
            {
                int temp = randy.Next(0, potentialDrops.Count);
                Debug.Log(temp);
                Instantiate(potentialDrops[temp], transform);
            }
        }
    }
}
