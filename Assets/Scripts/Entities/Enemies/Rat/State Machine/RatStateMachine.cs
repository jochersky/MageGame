using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class RatStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerEnteredSensor playerEnteredSensor;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private Health health;
    [SerializeField] private Hitbox hitbox;
    private Rigidbody2D _rb;
    private Collider2D _ownCollider;

    [Header("Move Properties")] 
    [SerializeField] private float defaultMoveSpeed = 3f;
    [SerializeField] private float aggroMoveSpeed = 6f;
    [SerializeField] private float lungeTime = 2f;
    [SerializeField] private float lungeVelocityX = 1;
    [SerializeField] private float lungeVelocityY = 1;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.25f);

    [Header("Wall Check")] 
    [SerializeField] private Vector2 wallCheckOffset;
    [SerializeField] private float wallCheckDistance = 0.75f;
    [SerializeField] private bool wallCheckDebug;

    [Header("Ledge Check")]
    [SerializeField] private float ledgeCheckDistance = 0.75f;
    [SerializeField] private bool ledgeCheckDebug;
    
    // State Variables
    private RatBaseState _currentState;
    private RatBaseState _currentSubState;
    private RatStateDictionary _states;

    // Animation Hashes
    public readonly int Grounded = Animator.StringToHash("Grounded");
    public readonly int Fall = Animator.StringToHash("Fall");
    public readonly int Lunge = Animator.StringToHash("Lunge");
    public readonly int Dead = Animator.StringToHash("Dead");
    
    private LayerMask _hitLayers;
    private float _currentMoveSpeed;
    private Vector2 _moveDir = Vector2.right;
    private float _horizontalMovement;
    private bool _isGrounded;
    private bool _isAggroed;
    private float _lungeTimer;
    private bool _isDead;
    
    // Event for flipping the transform.
    public UnityEvent<float> onDirectionChanged;

    // State Setters & Getters
    public RatBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public RatBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public RatStateDictionary States { get { return _states; } set { _states = value; } }
    public Animator Animator { get { return animator; } }
    public float LungeVelocityX { get { return lungeVelocityX; } }
    public float LungeVelocityY { get { return lungeVelocityY; } }
    public float LinearVelocityX { get { return _rb.linearVelocityX; } set { _rb.linearVelocityX = value; } }
    public float LinearVelocityY { get { return _rb.linearVelocityY; } set { _rb.linearVelocityY = value; } }
    public float HorizontalMovement { get { return _horizontalMovement; } set { _horizontalMovement = value; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool IsAggroed { get { return _isAggroed; } set { _isAggroed = value; } }
    public float LungeTime { get { return lungeTime; } set { lungeTime = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }

    private void Awake()
    {
        _currentMoveSpeed = defaultMoveSpeed;
        _horizontalMovement = _moveDir.x * _currentMoveSpeed;
        
        // State machine initial state setup
        _states = new RatStateDictionary(this);
        _currentState = _isGrounded ? States.Grounded(): States.Fall();
        _currentState.EnterState();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _ownCollider = GetComponent<Collider2D>();

        health.OnDeath += () =>
        {
            _isDead = true;
            hitbox.gameObject.SetActive(false);
            health.gameObject.SetActive(false);
        };

        playerEnteredSensor.OnPlayerSighted += () =>
        {
            if (_isDead) return;
            _currentMoveSpeed = aggroMoveSpeed;
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
            _isAggroed = true;
        };
        
        _hitLayers = LayerMask.GetMask("Character", "Environment");
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckHitWall();
        CheckForLedge();
        
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocityY);
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(groundCheckTransform.position, groundCheckSize, 0, environmentLayer);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Grounded check gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckTransform.position, groundCheckSize);
    }

    private void CheckHitWall()
    {
        if (_isDead) return;

        Vector2 start = (Vector2)transform.position + wallCheckOffset * Math.Sign(_moveDir.x);
        if (wallCheckDebug)
        {
            Debug.DrawRay(start, _moveDir * wallCheckDistance, Color.red);
        }
        RaycastHit2D hit = Physics2D.Raycast(start, _moveDir, wallCheckDistance, _hitLayers);
        if (hit && hit.collider != _ownCollider)
        {
            _moveDir = -_moveDir;
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDir.x));
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
        }
    }

    private void CheckForLedge()
    {
        if (!_isGrounded || _isDead) return;
        
        Vector2 start = (Vector2)transform.position + _moveDir * ledgeCheckDistance;
        if (ledgeCheckDebug)
        {
            Debug.DrawRay(start, Vector2.down, Color.red);
        }
        if (!Physics2D.Raycast(start, Vector2.down, 1f, environmentLayer))
        {
            _moveDir = -_moveDir;
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDir.x));
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
        }
    }
}
