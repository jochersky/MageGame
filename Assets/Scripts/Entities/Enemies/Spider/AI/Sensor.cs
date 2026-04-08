using System;
using UnityEngine;

// TODO: fully implement this class
public class Sensor : MonoBehaviour
{
    [SerializeField] Transform LineOfSightTransform;
    [SerializeField] float maxLineOfSightDistance;
    private LayerMask mask;

    public delegate void TargetChanged();
    public event TargetChanged OnTargetChanged;
    
    public bool IsTargetInRangeAndVisible { get; private set; }
    public Func<Vector2> TargetPosition = () => Vector2.zero;
    private Vector2 LastSeenPosition;

    private void Start()
    {
        if (!LineOfSightTransform) LineOfSightTransform = transform;
        
        mask = LayerMask.GetMask("Player", "Environment");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            // Vector2 dir = (player.transform.position - LineOfSightTransform.position).normalized;
            // RaycastHit2D hit = Physics2D.Raycast(LineOfSightTransform.position, dir, maxLineOfSightDistance, mask);
            // if (hit.collider.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
            // {
            //     TargetPosition = playerStateMachine.transform.position;
            //     IsTargetInRangeAndVisible = true;
            // }
            TargetPosition = () => player.transform.position;
            LastSeenPosition = player.transform.position;
            IsTargetInRangeAndVisible = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerStateMachine player))
        {
            IsTargetInRangeAndVisible = false;
            TargetPosition = () => LastSeenPosition;
            OnTargetChanged?.Invoke();
        }
    }
}
