using UnityEngine;
using UnityEngine.Events;
public class PlayerStompHitbox : MonoBehaviour
{
    public int damageAmt;
    [SerializeField] private float stompAngleLimit = 30f;
    [SerializeField] private bool debug;
    
    public UnityEvent onEnemyStomped;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            Vector2 collisionDirectionVector = (Vector2)transform.position - contactPoint;
            if (debug)
            {
                Debug.DrawRay(transform.position, collisionDirectionVector.normalized, Color.red, 10);
            }
            if (Vector2.Angle(Vector2.down, collisionDirectionVector.normalized) <= stompAngleLimit)
            {
                // Player gets jump boost
                onEnemyStomped?.Invoke();
                // Call any stomp logic for the corresponding hitbox
                hurtbox.Stomped(damageAmt);
            }
        }
    }
}
