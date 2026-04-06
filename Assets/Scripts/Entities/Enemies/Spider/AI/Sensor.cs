using System;
using UnityEngine;

// TODO: fully implement this class
public class Sensor : MonoBehaviour
{
    [SerializeField] Transform LineOfSightTransform;
    [SerializeField] float maxLineOfSightDistance;
    private LayerMask mask;
    
    public bool IsTargetInRangeAndVisible { get; private set; }
    public Vector2 TargetPosition { get; private set; }

    private void Start()
    {
        if (!LineOfSightTransform) LineOfSightTransform = transform;
        
        mask = LayerMask.GetMask("Player", "Environment");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            Vector2 dir = (player.transform.position - LineOfSightTransform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(LineOfSightTransform.position, dir, maxLineOfSightDistance, mask);
            if (hit.collider.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
            {
                TargetPosition = playerStateMachine.transform.position;
                IsTargetInRangeAndVisible = true;
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            IsTargetInRangeAndVisible = false;
        }
    }
}
