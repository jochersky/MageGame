using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

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
    [SerializeField] private float walkTime;
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
    private float _lungeTimer;
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
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool IsAggroed { get { return _isAggroed; } set { _isAggroed = value; } }
    public bool TookDamage => health.CurrentHealth < health.MaxHealth;
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public float EmergeTime => emergeTime;
    public float WalkTime => walkTime;
    public float DefaultMoveSpeed => defaultMoveSpeed;
    public float AggroMoveSpeed => aggroMoveSpeed;
}
