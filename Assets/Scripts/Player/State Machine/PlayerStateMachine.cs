using System;
using UnityEngine;
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
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Transform jumpCheckTransform;
    [SerializeField] private Vector2 jumpCheckSize = new Vector2(1f, 0.25f);
    [SerializeField] private LayerMask environmentLayer;
    
    // State Variables
    private PlayerBaseState _currentState;
    private PlayerBaseState _currentSubState;
    private PlayerStateDictionary _states;
    
    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isGrounded;
    
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }

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
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _horizontalMovement = context.ReadValue<Vector2>().x * maxMoveSpeed;
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            if (context.started || context.performed)
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
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(jumpCheckTransform.position, jumpCheckSize);
    }
}
