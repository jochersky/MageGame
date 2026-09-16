using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Barrel : MonoBehaviour
{
    [SerializeField] GameObject manaCapsule;
    [SerializeField] GameObject coin;
    [SerializeField] GameObject heart;
    [SerializeField] GameObject bomb;
    [SerializeField] int defaultChanceForNothing;
    [SerializeField] int defaultChanceForMana;
    [SerializeField] int defaultChanceForCoin;
    [SerializeField] int defaultChanceForHeart;
    [SerializeField] int defaultChanceForBomb;
    int chanceForNothing;
    int chanceForMana;
    int chanceForCoin;
    int chanceForHeart;
    int chanceForBomb;
    [SerializeField] Hurtbox hurtbox;
    [SerializeField] TemporaryEffect effect;
    private GameObject _player;
    readonly List<GameObject> potentialDrops = new();
    System.Random randy;
    void Awake()
    {
        FindAnyObjectByType<MapGenerator>().OnPlayerPlaced += (player) => {_player = player;};  
    }
    void Start()
    {
        chanceForBomb = defaultChanceForBomb;
        chanceForCoin = defaultChanceForCoin;
        chanceForHeart = defaultChanceForHeart;
        chanceForMana = defaultChanceForMana;
        chanceForNothing = defaultChanceForNothing;
        if (chanceForBomb + chanceForCoin + chanceForHeart + chanceForMana + chanceForNothing != 100) {
            Debug.Log("Error: Barrel drop rates do not sum to 100%");
        }
        hurtbox.OnDamageTaken += OnDestroyed;
        randy = new System.Random();
        potentialDrops.Add(manaCapsule);
        potentialDrops.Add(coin);
        potentialDrops.Add(heart);
        potentialDrops.Add(bomb);
    }

    void OnDestroyed(DamageProperties damageProperties)
    {
        RecalculateChances();
        Instantiate(effect, transform.position, quaternion.identity);
        int roll = randy.Next(1, 101); // 1-100
        if (roll >= chanceForNothing)
        {
            int index = -1;
            roll -= chanceForNothing;
            if (roll <= chanceForMana)
            {
                index = 0;
            } else
            {
                roll -= chanceForMana;
                if (roll <= chanceForCoin)
                {
                    index = 1;
                } else
                {
                    roll -= chanceForCoin;
                    if (roll <= chanceForHeart)
                    {
                        index = 2;
                    } else
                    {
                        index = 3;
                    }
                }
            }
            // if (potentialDrops[index].TryGetComponent<Health>(out Health health))
            // {
            //     health.spawnInvulnerable = true;
            // }
            Instantiate(potentialDrops[index], transform.position, quaternion.identity);
        }
        Destroy(gameObject);
    }

    private void RecalculateChances()
    {
        Health health = _player.GetComponent<Health>();
        float healthPct = (float)health.CurrentHealth / health.MaxHealth;
        SpellManager spellMan = _player.GetComponent<SpellManager>();
        float manaPct = (float)spellMan.Mana / spellMan.MaxMana;
        int goodPct = chanceForCoin + chanceForHeart + chanceForMana;
        chanceForHeart = (int)((1 - healthPct) * goodPct);
        chanceForMana = (int)((1 - manaPct) * goodPct);
        if (chanceForHeart + chanceForMana > goodPct)
        {
            chanceForMana = goodPct - chanceForHeart;
            chanceForCoin = 0;
        } else
        {
            chanceForCoin = goodPct - chanceForHeart - chanceForMana;
        }
        // print("Coin: " + chanceForCoin);
        // print("Mana: " + chanceForMana);
        // print("Heart: " + chanceForHeart);
    }
}
