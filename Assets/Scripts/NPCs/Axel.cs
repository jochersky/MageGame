using Unity.VisualScripting;
using UnityEngine;

public class Axel : NPC
{
    // lines
    [SerializeField] string[] greetingLines;
    [SerializeField] string[] reminderLines;
    [SerializeField] string[] noEnemiesLines;
    [SerializeField] string[] successLines;

    // mark data
    [SerializeField] Color markColor;
    [SerializeField] GameObject markGlow;
    [SerializeField] GameObject reward;
    private IndicatorManager _indicatorManager;
    private Enemy mark;
    private bool markSelected = false;
    private bool markDefeated = false;
    const int HEALTH_FACTOR = 2;
    const int DAMAGE_INCREASE = 1;

    protected override void PlayerSetup(Player _player)
    {
        base.PlayerSetup(_player);
        dialogue = greetingLines;
        _indicatorManager = FindAnyObjectByType<IndicatorManager>();
    }
    protected override void Talk()
    {
        if (!markSelected)
        {
            MarkEnemy();
        }
        if (markDefeated)
        {
            dialogue = successLines;
        }
        base.Talk();
    }

    private void OnMarkDeath()
    {
        markDefeated = true;
        _indicatorManager.ClearTargets();
    }

    private void MarkEnemy()
    {
        mark = FindAnyObjectByType<Enemy>();
        if (mark == null)
        {
            dialogue = noEnemiesLines;
            return;
        }
        Health enemyHealth = mark.GetComponent<Health>();
        if (enemyHealth == null)
        {
            dialogue = noEnemiesLines;
            return;
        }
        enemyHealth.MaxHealth *= HEALTH_FACTOR;
        enemyHealth.Heal(enemyHealth.MaxHealth);
        enemyHealth.OnDeath += OnMarkDeath;
        Hitbox hitbox = mark.GetComponentInChildren<Hitbox>();
        if (hitbox == null)
        {
            dialogue = noEnemiesLines;
            return;
        }
        hitbox.DamageAmt += DAMAGE_INCREASE;
        SpriteRenderer spriteRenderer = mark.GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            dialogue = noEnemiesLines;
            return;
        }
        spriteRenderer.color = markColor;
        Instantiate(markGlow, mark.transform);

        _indicatorManager.AddTarget(mark.gameObject);
        _indicatorManager.InstantiateIndicators();

        markSelected = true;
    }

    void GrantReward()
    {
        Instantiate(reward, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public override void CloseDialogue()
    {
        base.CloseDialogue();
        if (markDefeated)
        {
            GrantReward();
        } else
        {
            // after being spoken to the first time
            dialogue = reminderLines;
        }
    }

}
