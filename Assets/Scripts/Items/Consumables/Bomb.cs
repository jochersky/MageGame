using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Bomb : Consumable
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private ParticleSystem explosionRadius;
    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField, Range(0, 10)] private float explodeTime = 1;
    [SerializeField] private float flashSpeed = 1f;
    [SerializeField] TemporaryEffect explosionEffect;

    private CountdownTimer _particleLifetimeTimer;
    
    private float timer = 0f;
    private bool _followRb = true;
    
    private void Start()
    {
        damageFlash.OnDamageFlashComplete += () => damageFlash.StartFlash();
        
        _particleLifetimeTimer = new CountdownTimer(explosionRadius.main.startLifetime.constantMax);
        _particleLifetimeTimer.OnTimerStop += () => { Destroy(gameObject); };
        
        hitboxCollider.enabled = false;
        explosionCollider.enabled = false;
        
        damageFlash.StartFlash();
        StartCoroutine(InitiateExplode());
    }

    private void Update()
    {
        _particleLifetimeTimer.Tick(Time.deltaTime);
    }
    
    private void FixedUpdate()
    {
        if (!_followRb) return;
        
        explosionCollider.transform.position = rb.position;
        hitboxCollider.transform.position = rb.position;
        explosionRadius.transform.position = rb.position;
        explosionParticles.transform.position = rb.position;
    }

    private IEnumerator InitiateExplode()
    {
        timer = 0f;
        while (timer < explodeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        hitboxCollider.enabled = true;
        explosionCollider.enabled = true;
        
        sprite.enabled = false;
        _followRb = false;
        
        // start particle effects
        explosionRadius.Play();
        explosionParticles.Play();
        _particleLifetimeTimer.Start();
        
        Instantiate(explosionEffect, transform.position, quaternion.identity);
        
        yield return new WaitForSeconds(0.1f);
        
        hitboxCollider.enabled = false;
        explosionCollider.enabled = false;
        
        // Explode();
    }
}
