using System;
using UnityEngine;

public class RopeHandler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private BoxCollider2D boxCollider;
    
    private Collider2D[] _colliders = new Collider2D[5];
    private ContactFilter2D _contactFilter;
    private bool _ropeInBounds;
    private Rope _rope;

    private void Start()
    {
        _contactFilter = new ContactFilter2D();
        _contactFilter.SetLayerMask(layerMask);
        _contactFilter.useTriggers = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            _ropeInBounds = true;
        }
    }

    public Rope GetRope()
    {
        if (!_ropeInBounds) return null;

        Vector3 pointA = new Vector3(transform.position.x - boxCollider.size.x / 2, transform.position.y + boxCollider.size.y / 2, 0);
        Vector3 pointB = new Vector3(transform.position.x + boxCollider.size.x / 2, transform.position.y - boxCollider.size.y / 2, 0);
        // Debug.DrawLine(pointA, pointB, Color.red, 1f);

        int n = Physics2D.OverlapArea(pointA, pointB, _contactFilter, _colliders);
        if (n > 0)
        {
            foreach (Collider2D c in _colliders)
            {
                if (!c) continue;
                if (c.TryGetComponent<Rope>(out Rope rope))
                {
                    for (int i = 0; i < 5; i++) _colliders[i] = null;
                    return rope;
                }
            }
        }
        
        _ropeInBounds = false;
        
        return null;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Grounded check gizmo
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireCube(transform.position, boxCollider.size);
    }
}
