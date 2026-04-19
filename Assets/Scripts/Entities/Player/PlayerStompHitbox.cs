using System.Linq;
using UnityEngine;
using UnityEngine.Events;
public class PlayerStompHitbox : MonoBehaviour
{
    public int damageAmt;
    [SerializeField] private string[] tagsToIgnore;
    [SerializeField] private float stompAngleLimit = 30f;
    [SerializeField] private bool debug;
    
    public UnityEvent onEnemyStomped;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (tagsToIgnore.Contains(other.tag)) return;
        
        if (other.TryGetComponent<Hurtbox>(out Hurtbox hurtbox))
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);
            Vector2 collisionDirectionVector = (Vector2)transform.position - contactPoint;
            if (debug)
            {
                Debug.DrawRay(transform.position, Vector2.down, Color.red, 3);
                Debug.DrawRay(transform.position, collisionDirectionVector.normalized, Color.red, 3);
            }
            if (Vector2.Angle(Vector2.down, collisionDirectionVector.normalized) >= stompAngleLimit)
            {
                // Player gets jump boost
                onEnemyStomped?.Invoke();
                // Call any stomp logic for the corresponding hitbox
                hurtbox.Stomped(damageAmt);
            }
        }
    }
}
