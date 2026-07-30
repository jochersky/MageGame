using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class KnockBack : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float yReductionConst = 0.1f;
    
    private Health _health;

    public delegate void KnockBackApplied(Vector2 knockBackForce);
    public event KnockBackApplied OnKnockBackApplied;
    
    private void Start()
    {
        _health = GetComponent<Health>();

        _health.OnDamageTaken += ApplyKnockBackForceFromDamage;
    }

    private void ApplyKnockBackForce(Vector2 force)
    {
        force.y *= yReductionConst;
        OnKnockBackApplied?.Invoke(force);
    }

    private void ApplyKnockBackForceFromDamage(DamageProperties damageProperties)
    {
        ApplyKnockBackForce(damageProperties.direction * damageProperties.knockBackForce);
    }
}
