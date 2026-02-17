using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class RatStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnteredSensor playerEnteredSensor;
    [SerializeField] private LayerMask environmentLayer;
    private Rigidbody2D _rb;
    // private Animator _animator;

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
    [SerializeField] private float wallCheckDistance = 0.75f;
    [SerializeField] private bool wallCheckDebug;

    [Header("Ledge Check")]
    [SerializeField] private float ledgeCheckDistance = 0.75f;
    [SerializeField] private bool ledgeCheckDebug;
    
    // State Variables
    private RatBaseState _currentState;
    private RatBaseState _currentSubState;
    private RatStateDictionary _states;

    private float _currentMoveSpeed;
    private Vector2 _moveDir = Vector2.right;
    private float _horizontalMovement;
    private bool _isGrounded;
    private bool _isAggroed;
    private float _lungeTimer;
    
    // Event for flipping the transform.
    public UnityEvent<float> onDirectionChanged;

    // State Setters & Getters
    public RatBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public RatBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public RatStateDictionary States { get { return _states; } set { _states = value; } }
    
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }

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
        // _animator = GetComponent<Animator>();

        playerEnteredSensor.OnPlayerSighted += () =>
        {
            _currentMoveSpeed = aggroMoveSpeed;
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
            _isAggroed = true;
        };
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

        if (_isAggroed)
        {
            _lungeTimer += Time.fixedDeltaTime;
            if (_isGrounded && _lungeTimer >= lungeTime)
            {
                _rb.linearVelocity = new Vector2(lungeVelocityX * _moveDir.x, lungeVelocityY);
                _lungeTimer = 0;
            }
        }
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
        if (wallCheckDebug)
        {
            Debug.DrawRay(transform.position, _moveDir * wallCheckDistance, Color.red);
        }
        if (Physics2D.Raycast(transform.position, _moveDir, wallCheckDistance, environmentLayer))
        {
            _moveDir = -_moveDir;
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDir.x));
            _horizontalMovement = _moveDir.x * _currentMoveSpeed;
        }
    }

    private void CheckForLedge()
    {
        if (!_isGrounded) return;
        
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
