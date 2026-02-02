using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateMachine : MonoBehaviour
{
    // References
    private Rigidbody2D _rb;
    
    [Header("Movement Properties")]
    [SerializeField] private float maxWalkSpeed = 1f;
    [SerializeField] private float maxAirborneMoveSpeed = 1f;
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private Transform jumpCheckTransform;
    [SerializeField] private Vector2 jumpCheckSize = new Vector2(1f, 0.25f);
    [SerializeField] private LayerMask environmentLayer;

    [Header("Jump")] 
    [SerializeField] private float coyoteJumpTimer = 0.1f;
    
    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2;
    [SerializeField] private float maxFallSpeed = 15;
    [SerializeField] private float fallSpeedMultiplier = 1.5f;
    
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
    public bool _isPressingJump;

    public UnityEvent<float> onDirectionChanged;
    
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }
    
    public Vector2 MoveDirection { get { return _moveDirection; } set { _moveDirection = value; } }
    
    public Rigidbody2D Rigidbody { get { return _rb; } set { _rb = value; } }
    public Vector2 LinearVelocity { get { return _rb.linearVelocity; } set { _rb.linearVelocity = value; } }
    public float LinearVelocityX { get { return _rb.linearVelocityX; } set { _rb.linearVelocityX = value; } }
    public float LinearVelocityY { get { return _rb.linearVelocityY; } set { _rb.linearVelocityY = value; } }
    public float HorizontalMovement { get { return _horizontalMovement; } set { _horizontalMovement = value; } }
    public float VerticalMovement { get { return _verticalMovement; } set { _verticalMovement = value; } }
    public float GravityScale { get { return _rb.gravityScale; } set { _rb.gravityScale = value; } }
    
    public float MaxWalkSpeed { get { return maxWalkSpeed; } set { maxWalkSpeed = value; } }
    public float MaxAirborneMoveSpeed { get { return maxAirborneMoveSpeed; } set { maxAirborneMoveSpeed = value; } }
    public float MaxJumpHeight { get { return maxJumpHeight; } set { maxJumpHeight = value; } }
    public float BaseGravity { get { return baseGravity; } set { baseGravity = value; } }
    public float MaxFallSpeed { get { return maxFallSpeed; } set { maxFallSpeed = value; } }
    public float FallSpeedMultiplier { get { return fallSpeedMultiplier; } set { fallSpeedMultiplier = value; } }
    
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool CanJump { get { return _canJump; } set { _canJump = value; } }
    public bool IsPressingJump { get { return _isPressingJump; } set { _isPressingJump = value; } }

    private void Awake()
    {
        // State machine + initial state setup
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
        UpdateGravity();
        // _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocity.y);
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocityY);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
        // _horizontalMovement = _moveDirection.x * maxWalkSpeed;

        // Performed and canceled callbacks incorrectly flip the transform, ignore these.
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
        
        // if (_canJump)
        // {
        //     if (context.performed)
        //     {
        //         _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, maxJumpHeight);
        //     }
        //     else if (context.canceled)
        //     {
        //         _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
        //     }
        // }
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics2D.OverlapBox(jumpCheckTransform.position, jumpCheckSize, 0, environmentLayer);
        
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

    private void UpdateGravity()
    {
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(jumpCheckTransform.position, jumpCheckSize);
    }
}
