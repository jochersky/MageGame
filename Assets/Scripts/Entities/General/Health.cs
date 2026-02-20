using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private Hurtbox[] hurtboxes;
    
    [Header("Properties")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityTime = 1.5f;
    
    private int _currentHealth;
    private bool _isInvulnerable;
    
    public delegate void death();
    public event death OnDeath;
    
    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    private void Start()
    {
        foreach (Hurtbox hurtbox in hurtboxes)
        {
            hurtbox.OnDamageTaken += TakeDamage;
        }
    }

    private void TakeDamage(int damageAmt)
    {
        if (_isInvulnerable || _currentHealth <= 0) return;
        
        _currentHealth -= damageAmt;
        StartCoroutine(Invulnerable());
        if (damageFlash) damageFlash.StartFlash();
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }

    public void ActivateInvulnerability()
    {
        StartCoroutine(Invulnerable());
    }

    private IEnumerator Invulnerable()
    {
        _isInvulnerable = true;
        float timer = 0;
        while (timer < invulnerabilityTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _isInvulnerable = false;
    }
}
