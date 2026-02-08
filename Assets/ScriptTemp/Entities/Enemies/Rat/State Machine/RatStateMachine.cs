using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RatStateMachine : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private LayerMask environmentLayer;

    private Rigidbody2D _rb;

    [Header("Move Properties")] [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(1f, 0.25f);

    // State Variables
    private RatBaseState _currentState;
    private RatBaseState _currentSubState;
    private RatStateDictionary _states;

    private Vector2 _moveDir = Vector2.right;
    private float _horizontalMovement;
    private bool _isGrounded;

    // State Setters & Getters
    public RatBaseState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    public RatBaseState CurrentSubState
    {
        get { return _currentSubState; }
        set { _currentSubState = value; }
    }

    public RatStateDictionary States
    {
        get { return _states; }
        set { _states = value; }
    }

    public bool IsGrounded
    {
        get { return _isGrounded; }
        set { _isGrounded = value; }
    }

    private void Awake()
    {
        _horizontalMovement = _moveDir.x * moveSpeed;

        // State machine initial state setup
        _states = new RatStateDictionary(this);
        _currentState = _isGrounded ? States.Grounded(): States.Fall();
        _currentState.EnterState();
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        CheckHitWall();
        UpdateGravity();
        _rb.linearVelocity = new Vector2(_horizontalMovement, _rb.linearVelocityY);
    }

    private void UpdateGravity()
    {
        // // Player falls down faster with negative y-velocity.
        // if (_rb.linearVelocityY < 0)
        // {
        //     _rb.gravityScale = baseGravity * fallSpeedMultiplier;
        //     _rb.linearVelocityY = Mathf.Max(_rb.linearVelocityY, -maxFallSpeed);
        // }
        // else
        // {
        //     _rb.gravityScale = baseGravity;
        // }
        
        // _rb.gravityScale = baseGravity;
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
        Debug.DrawRay(transform.position, _moveDir, Color.red);
        if (Physics2D.Raycast(transform.position, _moveDir, 1f, environmentLayer))
        {
            _moveDir = -_moveDir;
        }
    }
}
