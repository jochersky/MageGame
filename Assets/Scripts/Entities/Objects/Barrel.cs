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
    [SerializeField] int chanceForNothing;
    [SerializeField] int chanceForEnemy;
    [SerializeField] int chanceForMana;
    [SerializeField] int chanceForCoin;
    [SerializeField] int chanceForHeart;
    [SerializeField] int chanceForBomb;
    [SerializeField] Hurtbox hurtbox;
    [SerializeField] TemporaryEffect effect;
    private int _enemyIndex = 0;
    readonly List<GameObject> potentialDrops = new();
    bool triggered = false;
    System.Random randy;
    void Start()
    {
        if (chanceForBomb + chanceForCoin + chanceForEnemy + chanceForHeart + chanceForMana + chanceForNothing != 100) {
            Debug.Log("Error: Barrel drop rates do not sum to 100%");
        }
        hurtbox.OnDamageTaken += OnDestroyed;
        randy = new System.Random();
        potentialDrops.Add(enemy);
        potentialDrops.Add(manaCapsule);
        potentialDrops.Add(coin);
        potentialDrops.Add(heart);
        potentialDrops.Add(bomb);
    }

    void OnDestroyed(DamageProperties damageProperties)
    {
        Instantiate(effect, transform.position, quaternion.identity);
        int roll = randy.Next(1, 101); // 1-100
        if (roll >= chanceForNothing)
        {
            int index = -1;
            roll -= chanceForNothing;
            if (roll <= chanceForEnemy)
            {
                index = 0;
            } else
            {
                roll -= chanceForEnemy;
                if (roll <= chanceForMana)
                {
                    index = 1;
                } else
                {
                    roll -= chanceForMana;
                    if (roll <= chanceForCoin)
                    {
                        index = 2;
                    } else
                    {
                        roll -= chanceForCoin;
                        if (roll <= chanceForHeart)
                        {
                            index = 3;
                        } else
                        {
                            index = 4;
                        }
                    }
                }
            }
            if (potentialDrops[index].TryGetComponent<Health>(out Health health))
            {
                health.spawnInvulnerable = true;
            }
            GameObject spawned = Instantiate(potentialDrops[index], transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }
}
