using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private Hurtbox[] hurtboxes;
    
    [Header("Properties")]
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private float invulnerabilityTime = 1.5f;
    [SerializeField] private bool capHealth = true;
    
    public int _currentHealth;
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    private bool _isInvulnerable;
    
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    
    public delegate void HealthChange(int newHealth);
    public event HealthChange OnHealthChanged;
    public delegate void Death();
    public event Death OnDeath;
    public delegate void Destroyed();
    public event Destroyed OnDestroyed;
    
    private void Awake()
    {
        _currentHealth = maxHealth;
    }
    
    private void Start()
    {
        foreach (Hurtbox hurtbox in hurtboxes)
        {
            hurtbox.OnDamageTaken += TakeDamage;
            hurtbox.OnHeal += Heal;
        }
    }

    private void TakeDamage(int damageAmt)
    {
        if (_isInvulnerable || _currentHealth <= 0) return;
        
        _currentHealth -= damageAmt;
        StartCoroutine(Invulnerable());
        
        OnHealthChanged?.Invoke(_currentHealth);
        if (damageFlash) damageFlash.StartFlash();
        if (_currentHealth < 0f) OnDestroyed?.Invoke();
        if (_currentHealth <= 0f) OnDeath?.Invoke();
    }

    public void Heal(int healAmt)
    {
        if (capHealth && _currentHealth + healAmt >= maxHealth) return;
        
        _currentHealth += healAmt;
        
        OnHealthChanged?.Invoke(_currentHealth);
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
