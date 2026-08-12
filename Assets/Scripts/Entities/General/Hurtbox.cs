using System;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] protected string ignoreTag;
    
    public delegate void damageTaken(DamageProperties damageProperties);
    public event damageTaken OnDamageTaken;
    public delegate void healed(int healAmt);
    public event healed OnHeal;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoreTag != "" && other.CompareTag(ignoreTag)) return;
        
        if (other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            DamageProperties dmgProps = hitbox.DamageProperties;
            // Add incoming damage direction
            DamageProperties damageProperties = new DamageProperties
            {
                amount = dmgProps.amount,
                cameraShakeProperties = dmgProps.cameraShakeProperties,
                direction = (transform.position - other.transform.position).normalized,
                knockBackForce = dmgProps.knockBackForce,
                effect = dmgProps.effect,
            };
            OnDamageTaken?.Invoke(damageProperties);
        }

        if (other.TryGetComponent<Healbox>(out Healbox healbox))
        {
            OnHeal?.Invoke(healbox.healAmt);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (ignoreTag != "" && other.CompareTag(ignoreTag)) return;
        
        if (other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            OnDamageTaken?.Invoke(hitbox.DamageProperties);
        }

        if (other.TryGetComponent<Healbox>(out Healbox healbox))
        {
            OnHeal?.Invoke(healbox.healAmt);
        }
    }

    public void Stomped(DamageProperties damageProperties)
    {
        OnDamageTaken?.Invoke(damageProperties);
    }
}
