using System;
using System.Collections;
using UnityEngine;

public class Bomb : Consumable
{
    [SerializeField] private Transform rbTransform;
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField, Range(0, 10)] private float explodeTime = 1;
    [SerializeField] private float flashSpeed = 1f;

    private float timer = 0f;
    
    private void Start()
    {
        damageFlash.OnDamageFlashComplete += () => damageFlash.StartFlash();
        
        hitboxCollider.enabled = false;
        explosionCollider.enabled = false;
        
        damageFlash.StartFlash();
        StartCoroutine(InitiateExplode());
    }
    
    private void FixedUpdate()
    {
        explosionCollider.transform.position = rbTransform.position;
        hitboxCollider.transform.position = rbTransform.position;
    }

    private void Explode()
    {
        Destroy(gameObject);
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
        yield return new WaitForSeconds(0.1f);
        Explode();
    }
}
