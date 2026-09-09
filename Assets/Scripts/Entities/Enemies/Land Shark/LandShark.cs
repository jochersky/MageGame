using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class LandShark : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Hurtbox hurtbox;
    [Header("Properties")]
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool debug;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Vector2 wallCheckDir = Vector2.left;
    [SerializeField] private Vector2 farWallCheckDir = Vector2.left;
    [SerializeField] private Vector2 lowerWallCheckDir = Vector2.right;
    [SerializeField] private Vector2 lowerWallCheckOffset = new(-1, -1);
    [SerializeField] private float diveSpeed = 1f;
    private Rigidbody2D _rb;
    private Health _health;
    
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
                _rotateCoroutine = StartCoroutine(RotateToNewNormal());
            }
            _normal = value;
        } 
    }
    private bool _isRotating = false;
    private Coroutine _rotateCoroutine;
    private bool _isDead = false;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = wallCheckDir * speed;
        
        _health = GetComponent<Health>();
        _health.OnDeath += () =>
        {
            _isDead = true;
            hurtbox.gameObject.SetActive(false);
            hitbox.gameObject.SetActive(false);
            _rb.linearVelocity = Vector2.zero;
            if (_rotateCoroutine != null) StopCoroutine(_rotateCoroutine);
            StartCoroutine(DeathAnimation());
        };
    }

    private void FixedUpdate()
    {
        if (_isRotating || _isDead) return;
        
        Vector2 pos = new(transform.position.x, transform.position.y);
        
        // Wall in path
        if (debug) Debug.DrawRay(pos, wallCheckDir, Color.red, 0.1f);
        RaycastHit2D hit = Physics2D.Raycast(pos, wallCheckDir, wallCheckDir.magnitude, layerMask);
        if (hit)
        {
            _hitPoint = hit.point;
            Normal = hit.normal;
            
            if (debug) Debug.DrawRay(hit.point, hit.normal, Color.purple, 3);
        }
        
        // Wall missing below
        if (debug) Debug.DrawRay(pos, farWallCheckDir, Color.red, 0.1f);
        hit = Physics2D.Raycast(pos, farWallCheckDir, farWallCheckDir.magnitude, layerMask);
        if (!hit)
        { 
            if (debug) Debug.DrawRay(pos + lowerWallCheckOffset, lowerWallCheckDir, Color.red, 0.1f);
            hit = Physics2D.Raycast(pos + lowerWallCheckOffset, lowerWallCheckDir, lowerWallCheckDir.magnitude, layerMask);
            if (hit)
            {
                _hitPoint = hit.point;
                Normal = hit.normal;
            }
        }
    }

    IEnumerator RotateToNewNormal()
    {
        _isRotating = true;
        
        Quaternion rot = Quaternion.FromToRotation(_prevNormal, Normal);
        
        // Move "down"
        Vector3 targetPos = transform.position - new Vector3(_prevNormal.x, _prevNormal.y, 0);
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, diveSpeed * Time.deltaTime);
            yield return null;
        }
        
        // Rotate transform
        float deg = rot.eulerAngles.z;
        Quaternion q = transform.rotation;
        q.eulerAngles = new(0, 0, transform.rotation.eulerAngles.z + deg);
        transform.rotation = q;
        
        // Move "up"
        targetPos = _hitPoint + Normal * 0.5f;
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, diveSpeed * Time.deltaTime);
            yield return null;
        }
        
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

        _isRotating = false;
    }

    IEnumerator DeathAnimation()
    {
        // Move "down"
        Vector3 targetPos = transform.position - new Vector3(Normal.x, Normal.y, 0);
        while (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, diveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
