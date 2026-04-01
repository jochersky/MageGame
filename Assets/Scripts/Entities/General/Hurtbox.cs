using System;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private string ignoreTag;
    
    public delegate void damageTaken(int damageAmt);
    public event damageTaken OnDamageTaken;
    public delegate void healed(int healAmt);
    public event healed OnHeal;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoreTag != "" && other.CompareTag(ignoreTag)) return;
        
        if (other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            OnDamageTaken?.Invoke(hitbox.damageAmt);
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
            OnDamageTaken?.Invoke(hitbox.damageAmt);
        }

        if (other.TryGetComponent<Healbox>(out Healbox healbox))
        {
            OnHeal?.Invoke(healbox.healAmt);
        }
    }

    public void Stomped(int damageAmt)
    {
        OnDamageTaken?.Invoke(damageAmt);
    }
}
