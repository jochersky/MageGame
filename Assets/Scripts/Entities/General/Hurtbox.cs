using System;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private string ignoreTag;
    
    public delegate void damageTaken(int damageAmt);
    public event damageTaken OnDamageTaken;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ignoreTag != "" && other.CompareTag(ignoreTag)) return;
        
        if (other.TryGetComponent<Hitbox>(out Hitbox hitbox))
        {
            OnDamageTaken?.Invoke(hitbox.damageAmt);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (ignoreTag != "" && other.CompareTag(ignoreTag)) return;
        
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
