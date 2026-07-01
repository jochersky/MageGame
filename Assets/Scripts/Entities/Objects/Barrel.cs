using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject enemy; 
    [SerializeField] GameObject manaCapsule;
    [SerializeField] GameObject coin;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject bomb;
    [SerializeField] Hurtbox hurtbox;
    [SerializeField] TemporaryEffect effect;
    private int _enemyIndex = 0;
    readonly List<GameObject> potentialDrops = new();
    bool triggered = false;
    System.Random randy;
    void Start()
    {
        hurtbox.OnDamageTaken += OnDestroyed;
        randy = new System.Random();
        potentialDrops.Add(enemy);
        potentialDrops.Add(manaCapsule);
        potentialDrops.Add(coin);
        potentialDrops.Add(heart);
        potentialDrops.Add(bomb);
    }

    void OnDestroyed(int _amt)
    {
        Instantiate(effect, transform.position, quaternion.identity);
        // Spawn random drop at 75% chance
        if (randy.Next(0, 100) < 75)
        {
            int temp = randy.Next(0, potentialDrops.Count);
            if (potentialDrops[temp].TryGetComponent<Health>(out Health health))
            {
                health.spawnInvulnerable = true;
            }
            GameObject spawned = Instantiate(potentialDrops[temp], transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }
}
