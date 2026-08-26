using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
[RequireComponent(typeof(Rigidbody2D))]

[RequireComponent(typeof(Collider2D))]
public class PlantStateMachine : Entity
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerEnteredSensor playerEnteredSensor;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private Health health;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Hurtbox hurtbox;
    [SerializeField] private KnockBack knockBack;
    private Rigidbody2D _rb;
    private Collider2D _ownCollider;
    
    [Header("Move Properties")] 
    [SerializeField] private float emergeTime;
    [SerializeField] private float aggroTime = 4f;
    [SerializeField] private float defaultMoveSpeed = 3f;
    [SerializeField] private float aggroMoveSpeed = 6f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.25f);

    [Header("Wall Check")] 
    [SerializeField] private Vector2 wallCheckOffset;
    [SerializeField] private float wallCheckDistance = 0.75f;
    [SerializeField] private bool wallCheckDebug;

    [Header("Ledge Check")]
    [SerializeField] private int fallChance = 5;
    [SerializeField] private float ledgeCheckDistance = 0.75f;
    [SerializeField] private bool ledgeCheckDebug;
    
    [Header("Knock Back")]
    [SerializeField, Range(0, 1)] private float reduceConst = 0.3f;
    
    [Header("State Debug")]
    public string stateName = "";
    
    // State Variables
    private PlantBaseState _currentState;
    private PlantBaseState _currentSubState;
    private PlantStateDictionary _states;

    // Animation Hashes
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Emerge = Animator.StringToHash("Emerge");
    public readonly int Hide = Animator.StringToHash("Hide");
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Aggro = Animator.StringToHash("Aggro");
    public readonly int Fall = Animator.StringToHash("Fall");
    public readonly int Dead = Animator.StringToHash("Dead");
    
    private LayerMask _hitLayers;
    private float _currentMoveSpeed;
    private Vector2 _moveDir = Vector2.right;
    private float _horizontalMovement;
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isAggroed;
    private bool _isDead;
    private RaycastHit2D[] _hits = new RaycastHit2D[3];
    private bool _fallIntentionally = false;
    private Vector2 _knockBackForce;
    
    // Event for flipping the transform.
    public UnityEvent<float> onDirectionChanged;

    // State Setters & Getters
    public PlantBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlantBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlantStateDictionary States { get { return _states; } set { _states = value; } }
    public Animator Animator { get { return animator; } }
    public Vector2 MoveDir => _moveDir;
    public float LinearVelocityX { get { return _rb.linearVelocityX; } set { _rb.linearVelocityX = value; } }
    public float LinearVelocityY { get { return _rb.linearVelocityY; } set { _rb.linearVelocityY = value; } }
    public float HorizontalMovement { get { return _horizontalMovement; } set { _horizontalMovement = value; } }
    public float CurrentMoveSpeed { get { return _currentMoveSpeed; }  set { _currentMoveSpeed = value; } }
    public float CurrentHealth { get { return health.CurrentHealth; } set {health.CurrentHealth = health.MaxHealth; }}
    public float MaxHealth { get { return health.MaxHealth; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool IsAggroed { get { return _isAggroed; } set { _isAggroed = value; } }
    public bool TookDamage => health.CurrentHealth < health.MaxHealth;
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public float EmergeTime => emergeTime;
    public float AggroTime => aggroTime;
    public float DefaultMoveSpeed => defaultMoveSpeed;
    public float AggroMoveSpeed => aggroMoveSpeed;
     private void Awake()
    {
        // State machine initial state setup
        _states = new PlantStateDictionary(this);
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
            hurtbox.gameObject.SetActive(false);
        };

        playerEnteredSensor.OnPlayerSighted += () =>
        {
            if (_isDead) return;
            _currentMoveSpeed = aggroMoveSpeed;
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
            _isAggroed = true;
        };
        
        knockBack.OnKnockBackApplied += force => { _knockBackForce = force; }; 
        
        _hitLayers = LayerMask.GetMask("Character", "Environment");
    }

    private void Update()
    {
        _currentState.UpdateStates();
        stateName = _currentState.SubState != null ? _currentState.SubState.ToString() : _currentState.ToString();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckHitWall();
        CheckForLedge();
        
        // freeze spell behavior, can be used with other effects in the future
        if (frozen) return;
        
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocityY);
        _rb.linearVelocity += _knockBackForce;

        UpdateKnockBackForce();
    }
    
    private void UpdateKnockBackForce()
    {
        _knockBackForce *= reduceConst;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<ColdSnapArea>(out ColdSnapArea coldSnap))
        {
            Freeze(coldSnap.freezeDuration);
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(groundCheckTransform.position, groundCheckSize, 0, environmentLayer);

        if (!_wasGrounded && _isGrounded) _fallIntentionally = false;
        
        _wasGrounded = _isGrounded;
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
        int numHits = Physics2D.RaycastNonAlloc(start, _moveDir, _hits, wallCheckDistance, _hitLayers);
        if (numHits > 1)
        {
            Array.Sort(_hits, (a, b) => a.distance.CompareTo(b.distance));
        }
        for (int i = 0; i < numHits; i++)
        {
            RaycastHit2D hit = _hits[i];
            if (!hit) break;
            if (hit.collider != _ownCollider)
            {
                _moveDir = -_moveDir;
                onDirectionChanged?.Invoke(Mathf.Sign(_moveDir.x));
                _horizontalMovement = _moveDir.x * _currentMoveSpeed;
                break;
            }
        }
    }

    private void CheckForLedge()
    {
        if (!_isGrounded || _isDead || _fallIntentionally) return;
        
        Vector2 start = (Vector2)transform.position + _moveDir * ledgeCheckDistance;
        if (ledgeCheckDebug)
        {
            Debug.DrawRay(start, Vector2.down, Color.red);
        }
        if (!Physics2D.Raycast(start, Vector2.down, 1f, environmentLayer))
        {
            if (Random.Range(0, fallChance) == 0) _fallIntentionally = true;
                
            _moveDir = -_moveDir;
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDir.x));
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
        }
    }
}
