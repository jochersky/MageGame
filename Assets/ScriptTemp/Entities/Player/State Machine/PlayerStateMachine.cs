using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask environmentLayer;
    private Rigidbody2D _rb;
    
    [Header("Walk")]
    [SerializeField] private float maxWalkSpeed = 1f;
    
    [Header("Jump")] 
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private Transform jumpCheckTransform;
    [SerializeField] private Vector2 jumpCheckSize = new Vector2(1f, 0.25f);
    [SerializeField] private float coyoteJumpTimer = 0.1f;
    
    [Header("Airborne")]
    [SerializeField] private float maxAirborneMoveSpeed = 1f;
    
    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2;
    [SerializeField] private float maxFallSpeed = 15;
    [SerializeField] private float fallSpeedMultiplier = 1.5f;
    
    [Header("Climbing")]
    [SerializeField] private Vector2 climbCheckOffset = Vector2.zero;
    [SerializeField] private Vector2 climbCheckDir = Vector2.right;
    [SerializeField] private float climbCheckDistance = 0.2f;
    [SerializeField] private bool climbDebug;
    
    // State Variables
    private PlayerBaseState _currentState;
    private PlayerBaseState _currentSubState;
    private PlayerStateDictionary _states;

    private Vector2 _moveDirection;
    private Vector2 _previousDirection;
    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isGrounded;
    private float _airTime;
    private bool _canJump;
    private bool _isPressingJump;
    private bool _canClimb;
    private bool _wasClimbing;

    // Event for flipping the transform.
    public UnityEvent<float> onDirectionChanged;
    
    // State Setters & Getters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }
    
    // Instance Variables + References Setters & Getters
    public Rigidbody2D Rigidbody { get { return _rb; } set { _rb = value; } }
    public Vector2 MoveDirection { get { return _moveDirection; } set { _moveDirection = value; } }
    public Vector2 LinearVelocity { get { return _rb.linearVelocity; } set { _rb.linearVelocity = value; } }
    public float LinearVelocityX { get { return _rb.linearVelocityX; } set { _rb.linearVelocityX = value; } }
    public float LinearVelocityY { get { return _rb.linearVelocityY; } set { _rb.linearVelocityY = value; } }
    public float HorizontalMovement { get { return _horizontalMovement; } set { _horizontalMovement = value; } }
    public float GravityScale { get { return _rb.gravityScale; } set { _rb.gravityScale = value; } }
    public float MaxWalkSpeed { get { return maxWalkSpeed; } set { maxWalkSpeed = value; } }
    public float MaxAirborneMoveSpeed { get { return maxAirborneMoveSpeed; } set { maxAirborneMoveSpeed = value; } }
    public float MaxJumpHeight { get { return maxJumpHeight; } set { maxJumpHeight = value; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool CanJump { get { return _canJump; } set { _canJump = value; } }
    public bool IsPressingJump { get { return _isPressingJump; } set { _isPressingJump = value; } }
    public bool CanClimb { get { return _canClimb; } set { _canClimb = value; } }
    public bool WasClimbing { get { return _wasClimbing; } set { _wasClimbing = value; } }

    private void Awake()
    {
        // State machine initial state setup
        _states = new PlayerStateDictionary(this);
        _currentState = _isGrounded ? _states.Grounded() : _states.Fall();
        _currentState.EnterState();
    }
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }
    
    void FixedUpdate()
    {
        CheckGrounded();
        CheckClimbing();
        UpdateGravity();
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocityY);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();

        // Performed and canceled callbacks incorrectly flip the transform. Ignore them.
        if (context.performed || context.canceled) return; 
        if (Mathf.Sign(_moveDirection.x) != Mathf.Sign(_previousDirection.x))
        {
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDirection.x));
        }
        _previousDirection = _moveDirection;
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        _isPressingJump = context.ReadValueAsButton();
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(jumpCheckTransform.position, jumpCheckSize, 0, environmentLayer);
        
        // Allow player extra time to jump after not being grounded.
        if (!_isGrounded)
        {
            _airTime += Time.deltaTime;
            _canJump = _airTime < coyoteJumpTimer;
        }
        else
        {
            _airTime = 0;
            _canJump = true;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Grounded check gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(jumpCheckTransform.position, jumpCheckSize);
    }

    private void UpdateGravity()
    {
        if (_currentState == _states.Climb()) return;
        
        // Player falls down faster with negative y-velocity.
        if (_rb.linearVelocityY < 0)
        {
            _rb.gravityScale = baseGravity * fallSpeedMultiplier;
            _rb.linearVelocityY = Mathf.Max(_rb.linearVelocityY, -maxFallSpeed);
        }
        else
        {
            _rb.gravityScale = baseGravity;
        }
    }

    public void EnemyStomped()
    {
        _rb.linearVelocityY = maxJumpHeight;
    }

    private void CheckClimbing()
    {
        float dirSign = Mathf.Sign(_previousDirection.x);
        Vector2 start = (Vector2)transform.position + climbCheckOffset;
        Vector2 direction = climbCheckDir * dirSign;

        if (climbDebug)
        {
            Debug.DrawRay(start, direction * climbCheckDistance, Color.orange);
            Debug.DrawRay(start + Vector2.up, direction * climbCheckDistance, Color.orange);
        }

        _canClimb = !_isGrounded 
                    && Physics2D.Raycast(start, direction, climbCheckDistance, environmentLayer)
                    && !Physics2D.Raycast(start + Vector2.up, direction, climbCheckDistance, environmentLayer);
    }
}
