using UnityEngine;
using UnityEngine.Events;

public class PlayerEnteredSensor : MonoBehaviour
{
    [SerializeField] private Vector2 rayVector = Vector2.right;
    [SerializeField] private bool disableOnEnter = false;
    [SerializeField] private bool debug;

    private LayerMask _characterLayer;
    private LayerMask _environmentLayer;
    private LayerMask _hitLayers;
    private RaycastHit2D[] _hits = new RaycastHit2D[5];
    private bool _playerFound = false;
    
    public delegate void PlayerSighted();
    public event PlayerSighted OnPlayerSighted;

    private void Start()
    {
        _hitLayers = LayerMask.GetMask("Character", "Environment");
    }

    private void FixedUpdate()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        if (_playerFound) return;
        
        if (debug) Debug.DrawRay(transform.position, rayVector * transform.localScale.x, Color.purple);
        
        Physics2D.RaycastNonAlloc(transform.position, rayVector * transform.localScale.x, _hits, rayVector.magnitude, _hitLayers);
        foreach (RaycastHit2D hit in _hits)
        {
            if (!hit) break;
            
            if (hit.collider.gameObject.name == "Player")
            {
                OnPlayerSighted?.Invoke();
                if (disableOnEnter) _playerFound = true;
            }
        }
    }
}
