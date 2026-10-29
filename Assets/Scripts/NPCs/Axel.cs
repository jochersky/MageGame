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
    private Enemy mark;
    private bool markSelected = false;
    private bool markDefeated = false;
    const int HEALTH_FACTOR = 2;
    const int DAMAGE_INCREASE = 1;

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
        // after being spoken to the first time
        dialogue = reminderLines;
    }

    private void OnMarkDeath()
    {
        markDefeated = true;
    }

    private void MarkEnemy()
    {
        mark = FindAnyObjectByType<Enemy>();
        Health enemyHealth = mark.GetComponent<Health>();
        enemyHealth.MaxHealth *= HEALTH_FACTOR;
        enemyHealth.OnDeath += OnMarkDeath;
        Hitbox hitbox = mark.GetComponentInChildren<Hitbox>();
        hitbox.damageAmt += DAMAGE_INCREASE;
        SpriteRenderer spriteRenderer = mark.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.color = markColor;
        Instantiate(markGlow, mark.transform);
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
        }
    }

}
