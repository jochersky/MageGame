using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class LandShark : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool debug;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector2 wallCheckDir = Vector2.left;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Vector2 wallCheckOffset;
    [SerializeField] private Vector2 farWallCheckDir = Vector2.left;
    [SerializeField] private Vector2 lowerWallCheckDir = Vector2.right;
    [SerializeField] private Vector2 lowerWallCheckOffset = new Vector2(-1, -1);
    private Rigidbody2D _rb;
    private Vector2 _hitPoint;
    private Vector2 _prevNormal = Vector2.zero;
    private Vector2 _normal = Vector2.up;
    private Vector2 Normal { 
        get => _normal;
        set
        {
            if (value != _normal)
            {
                _prevNormal = _normal;
                _normal = value;
                RotateWithNormal();
            }
            _normal = value;
        } 
    }
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = wallCheckDir * speed;
    }

    private void FixedUpdate()
    {
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Debug.DrawRay(pos, wallCheckDir * wallCheckDistance, Color.red, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(pos, wallCheckDir, wallCheckDistance, layerMask);
        
        if (hit)
        {
            _hitPoint = hit.point;
            Normal = hit.normal;
            
            Debug.DrawRay(hit.point, hit.normal, Color.purple, 3);
        }
        
        Debug.DrawRay(pos, farWallCheckDir, Color.red, 0.1f);
        hit = Physics2D.Raycast(pos, farWallCheckDir, farWallCheckDir.magnitude, layerMask);
        if (!hit)
        { 
            Debug.DrawRay(pos + lowerWallCheckOffset, lowerWallCheckDir, Color.red, 0.1f);
            hit = Physics2D.Raycast(pos + lowerWallCheckOffset, lowerWallCheckDir, lowerWallCheckDir.magnitude, layerMask);
            if (hit)
            {
                _hitPoint = hit.point;
                Normal = hit.normal;
            }
        }
    }

    private void RotateWithNormal()
    {
        Quaternion rot = Quaternion.FromToRotation(_prevNormal, Normal);
        // Rotate transform
        float deg = rot.eulerAngles.z;
        Quaternion q = transform.rotation;
        q.eulerAngles = new Vector3(0, 0, transform.rotation.eulerAngles.z + deg);
        transform.rotation = q;
        // Rotate velocity vector
        Vector3 newVelocity = rot * wallCheckDir * speed;
        _rb.linearVelocity = newVelocity;
        // Rotate close wall raycast vector
        Vector2 newWallCheckDir = rot * wallCheckDir;
        wallCheckDir = newWallCheckDir;
        // Rotate far wall raycast vector
        Vector2 newFarWallCheckDir = rot * farWallCheckDir;
        farWallCheckDir = newFarWallCheckDir;
        // Rotate lower wall raycast vector
        Vector2 newLowerWallCheckDir = rot * lowerWallCheckDir;
        lowerWallCheckDir = newLowerWallCheckDir;
        Vector2 newLowerWallCheckOffset = rot * lowerWallCheckOffset;
        lowerWallCheckOffset = newLowerWallCheckOffset;
        
        transform.position = _hitPoint + Normal * 0.5f;

        // Rotate offset vector
        // Vector2 newWallCheckOffset = rot * wallCheckOffset;
        // wallCheckOffset = newWallCheckOffset;
    }
}
