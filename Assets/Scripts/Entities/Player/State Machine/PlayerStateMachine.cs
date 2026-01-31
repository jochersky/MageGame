using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    private Rigidbody2D _rb;
    
    [Header("Movement Properties")]
    [SerializeField] private float maxMoveSpeed = 1f;
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private float moveAccel = 0.5f;
    [SerializeField] private float stopDrag = 0.6f;
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
    
    private Vector2 _previousDirection;
    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isGrounded;
    private float _airTime;
    private bool _canJump;

    public UnityEvent<float> onDirectionChanged;
    
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }
    public Rigidbody2D Rigidbody { get { return _rb; } set { _rb = value; } }

    private void Awake()
    {
        // State machine + initial state setup
        // _states = new PlayerStateDictionary(this);
        // _currentState = _states.Grounded();
        // _currentState.EnterState();
    }
    
    void Start()
    {
        CheckGrounded();
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        UpdateGravity();
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        _horizontalMovement = direction.x * maxMoveSpeed;

        if (context.performed || context.canceled) return; 
        
        if (Mathf.Sign(direction.x) != Mathf.Sign(_previousDirection.x))
        {
            onDirectionChanged?.Invoke(Mathf.Sign(direction.x));
        }
        _previousDirection = direction;
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (_canJump)
        {
            if (context.performed)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, maxJumpHeight);
            }
            else if (context.canceled)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
            }
        }
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
