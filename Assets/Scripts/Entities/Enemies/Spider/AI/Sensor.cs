using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

// TODO: fully implement this class
public class Sensor : MonoBehaviour
{
    [SerializeField] Transform LineOfSightTransform;
    [SerializeField] float maxLineOfSightDistance;
    private LayerMask _mask;
    private LayerMask _environmentLayer;

    public delegate void TargetChanged(GameObject target);
    public event TargetChanged OnTargetChanged;
    
    public bool IsTargetInRangeAndVisible { get; private set; }
    public Func<Vector2> TargetPosition = () => Vector2.zero;
    private Vector2 LastSeenPosition;
    public GameObject _currentTarget;
    private HashSet<GameObject> _nextTargets;
    // private RaycastHit2D[] hits = new RaycastHit2D[5];

    private void Start()
    {
        if (!LineOfSightTransform) LineOfSightTransform = transform;
        
        _nextTargets = new HashSet<GameObject>();
        
        // _mask = LayerMask.GetMask("Player", "Environment");
        // _environmentLayer = LayerMask.GetMask("Environment");
    }
    
    // TODO: May later be necessary
    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.TryGetComponent(out PlayerStateMachine player))
    //     {
    //         Vector2 dir = (player.transform.position - LineOfSightTransform.position).normalized;
    //         Physics2D.RaycastNonAlloc(LineOfSightTransform.position, dir, hits, maxLineOfSightDistance, _mask);
    //         Debug.DrawRay(LineOfSightTransform.position, dir * maxLineOfSightDistance, Color.red, 5);
    //
    //         var sortedHits = hits.OrderBy(a => a.distance);
    //         
    //         foreach (RaycastHit2D hit in sortedHits)
    //         {
    //             if (!hit) continue;
    //             if (Math.Pow(2, hit.collider.gameObject.layer) == _environmentLayer.value) break;
    //             if (hit.collider.gameObject.TryGetComponent<PlayerStateMachine>(out PlayerStateMachine playerStateMachine))
    //             {
    //                 TargetPosition = () => player.transform.position;
    //                 LastSeenPosition = player.transform.position;
    //                 IsTargetInRangeAndVisible = true;
    //                 OnTargetChanged?.Invoke();
    //             }
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerStateMachine player))
        {
            if (player.IsDead) return;
            
            if (!_currentTarget && _nextTargets.Count < 1) 
                HandleNewTargetSet(other.gameObject);
            else
                _nextTargets.Add(other.gameObject);
        }
        else if (other.TryGetComponent(out RatStateMachine rat))
        {
            if (rat.IsDead) return;
            
            if (!_currentTarget && _nextTargets.Count < 1) 
                HandleNewTargetSet(other.gameObject);
            else 
                _nextTargets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerStateMachine player))
        {
            if (other.gameObject == _currentTarget)
            {
                if (_nextTargets.Count < 1)
                {
                    IsTargetInRangeAndVisible = false;
                    TargetPosition = () => LastSeenPosition;
                    _currentTarget = null;
                    OnTargetChanged?.Invoke(null);
                }
                else
                {
                    var targetList = _nextTargets.ToList();
                    HandleNewTargetSet(targetList[0]);
                }
            }
            else
            {
                _nextTargets.Remove(other.gameObject);
            }
        }
        else if (other.TryGetComponent(out RatStateMachine rat))
        {
            if (other.gameObject == _currentTarget)
            {
                if (_nextTargets.Count < 1)
                {
                    IsTargetInRangeAndVisible = false;
                    TargetPosition = () => LastSeenPosition;
                    _currentTarget = null;
                    OnTargetChanged?.Invoke(null);
                }
                else
                {
                    var targetList = _nextTargets.ToList();
                    HandleNewTargetSet(targetList[0]);
                }
            }
            else
            {
                _nextTargets.Remove(other.gameObject);
            }
        }
    }

    private void HandleNewTargetSet(GameObject target)
    {
        if (_nextTargets.Contains(target)) _nextTargets.Remove(target);
        _currentTarget = target;
        TargetPosition = () => target.transform.position;
        LastSeenPosition = target.transform.position;
        IsTargetInRangeAndVisible = true;
        OnTargetChanged?.Invoke(target);
    }

    public void RemoveCurrentTarget()
    {
        _currentTarget = null;
        if (_nextTargets.Count < 1) return;
        
        var targetList = _nextTargets.ToList();
        // _currentTarget = targetList[0];
        // _nextTargets.Remove(targetList[0]);
        // TargetPosition = () => targetList[0].transform.position;
        // LastSeenPosition = targetList[0].transform.position;
        // IsTargetInRangeAndVisible = true;
        // OnTargetChanged?.Invoke(targetList[0]);
        HandleNewTargetSet(targetList[0]);
    }
}
