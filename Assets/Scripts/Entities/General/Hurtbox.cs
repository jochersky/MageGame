using System;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public delegate void damageTaken(int damageAmt);
    public event damageTaken OnDamageTaken;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            OnDamageTaken?.Invoke(hitbox.damageAmt);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            OnDamageTaken?.Invoke(hitbox.damageAmt);
        }
    }

    public void Stomped(int damageAmt)
    {
        OnDamageTaken?.Invoke(damageAmt);
    }
}
