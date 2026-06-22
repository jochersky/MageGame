using System;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private LayerMask environmentLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private Health health;
    [SerializeField] private PassiveSpellAffects passiveSpellAffects;
    [SerializeField] private BaseStats baseStats;
    private Rigidbody2D _rb;
    private InputActionMap _playerInputMap;
    private Stats _stats;
    private CameraManager _cameraManager;
    
    [Header("Walk")]
    [SerializeField] private float maxWalkSpeed = 1f;
    
    [Header("Jump")] 
    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private float maxDoubleJumpHeight = 10f;
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
    [SerializeField, Range(0.25f, 2)] private float climbDistanceFromWall = 1.25f;
    [SerializeField] private Vector2 climbCheckOffset = Vector2.zero;
    [SerializeField] private Vector2 climbCheckDir = Vector2.right;
    [SerializeField] private float climbCheckDistance = 0.2f;
    [SerializeField] private float climbCheckHeight = 0.7f;
    [SerializeField] private float climbAboveBelowCheckLength = 0.5f;
    [SerializeField] private float climbDelayTime = 0.1f;
    [SerializeField] private bool climbDebug;
    
    [Header("Ladder")]
    [SerializeField] private float ropeClimbSpeed = 0.25f;
    
    // State Variables
    private PlayerBaseState _currentState;
    private PlayerBaseState _currentSubState;
    private PlayerStateDictionary _states;

    // Animation Hashes
    public readonly int Idle = Animator.StringToHash("Idle");
    public readonly int Walk = Animator.StringToHash("Walk");
    public readonly int Jump = Animator.StringToHash("Jump");
    public readonly int Fall = Animator.StringToHash("Fall");
    public readonly int Climbing = Animator.StringToHash("Climb");
    public readonly int Dead = Animator.StringToHash("Dead");
    
    // Context Variables
    private Vector2 _moveDirection;
    private Vector2 _verticalDirection;
    private Vector2 _previousDirection;
    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _isGrounded;
    private float _airTime;
    private bool _canJump;
    private bool _justPressedJump;
    private bool _isPressingJump;
    private bool _newJumpPress;
    private int _numDoubleJumps;
    private bool _canClimb;
    private bool _wasClimbing;
    private Vector2 _climbPosition;
    private float _climbDelayTimer;
    private bool _climbCooldown;
    // private Tilemap _climbingTilemap;
    private bool _isDead;
    private bool _inputDisabled;
    private bool _canClimbRope;
    private bool _isClimbingRope;
    private bool _wasClimbingRope;
    private float _yRopeMin;
    private float _yRopeMax;
    private bool _isCrouching;

    [Header("State Debug")]
    public String stateName = "";

    // Event for flipping the transform.
    public UnityEvent<float> onDirectionChanged;

    public delegate void DoubleJumpComplete();
    public event DoubleJumpComplete OnDoubleJumpComplete;
    
    // State Setters & Getters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public PlayerStateDictionary States { get { return _states; } set { _states = value; } }
    
    // Instance Variables + References Setters & Getters
    public Rigidbody2D Rigidbody { get { return _rb; } set { _rb = value; } }
    public Stats Stats { get { return _stats; } set { _stats = value; } }
    public Animator Animator { get { return animator; } set { animator = value; } }
    public Vector2 MoveDirection { get { return _moveDirection; } set { _moveDirection = value; } }
    public Vector2 VerticalDirection { get { return _verticalDirection; } set { _verticalDirection = value; } }
    public Vector2 PreviousDirection { get { return _previousDirection; } set { _previousDirection = value; } }
    public Vector2 LinearVelocity { get { return _rb.linearVelocity; } set { _rb.linearVelocity = value; } }
    public float LinearVelocityX { get { return _rb.linearVelocityX; } set { _rb.linearVelocityX = value; } }
    public float LinearVelocityY { get { return _rb.linearVelocityY; } set { _rb.linearVelocityY = value; } }
    public float HorizontalMovement { get { return _horizontalMovement; } set { _horizontalMovement = value; } }
    public float VerticalMovement { get { return _verticalMovement; } set { _verticalMovement = value; } }
    public float GravityScale { get { return _rb.gravityScale; } set { _rb.gravityScale = value; } }
    public float MaxWalkSpeed { get { return maxWalkSpeed; } set { maxWalkSpeed = value; } }
    public float MaxAirborneMoveSpeed { get { return maxAirborneMoveSpeed; } set { maxAirborneMoveSpeed = value; } }
    public float MaxJumpHeight { get { return maxJumpHeight; } set { maxJumpHeight = value; } }
    public float MaxDoubleJumpHeight { get { return maxDoubleJumpHeight; } set { maxDoubleJumpHeight = value; } }
    public bool IsGrounded { get { return _isGrounded; } set { _isGrounded = value; } }
    public bool CanJump { get { return _canJump; } set { _canJump = value; } }
    public int NumDoubleJumps { get { return _numDoubleJumps; } set { _numDoubleJumps = value; } }
    public bool JustPressedJump { get { return _justPressedJump; } set { _justPressedJump = value; } }
    public bool IsPressingJump { get { return _isPressingJump; } set { _isPressingJump = value; } }
    public bool NewJumpPress { get { return _newJumpPress; } set { _newJumpPress = value; } }
    public bool CanClimb { get { return _canClimb; } set { _canClimb = value; } }
    public bool WasClimbing { get { return _wasClimbing; } set { _wasClimbing = value; } }
    public Vector2 ClimbPosition { get { return _climbPosition; } set { _climbPosition = value; } }
    public bool CanClimbRope { get { return _canClimbRope; } set { _canClimbRope = value; } }
    public bool IsClimbingRope { get { return _isClimbingRope; } set { _isClimbingRope = value; } }
    public bool WasClimbingRope { get {return _wasClimbingRope; }  set { _wasClimbingRope = value; } }
    public bool IsCrouching { get { return _isCrouching; } set { _isCrouching = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInputMap = playerInput.actions.actionMaps[0];
        _stats = new Stats(new StatsMediator(), baseStats);
        _cameraManager = GetComponentInChildren<CameraManager>();
        
        // Passive spell affects initialization
        _numDoubleJumps = passiveSpellAffects.doubleJumps;

        health.OnDeath += () => { _isDead = true; };
        
        // State machine initial state setup
        _states = new PlayerStateDictionary(this);
        _currentState = _isGrounded ? _states.Grounded() : _states.Jump();
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
        _stats.Mediator.Update(Time.deltaTime);
        stateName = _currentState.ToString();
    }
    
    void FixedUpdate()
    {
        CheckGrounded();
        CheckClimbing();
        UpdateGravity();

        float x = _horizontalMovement;
        float y = _rb.linearVelocityY;
        // lock player onto ladder horizontally until they jump off
        if (_isClimbingRope)
        {
            x = 0;
            
            // player should not be able to "leave" rope by descending or ascending it
            float posY = Mathf.Clamp(_rb.position.y, _yRopeMin, _yRopeMax);
            bool inBounds = posY > _yRopeMin && posY < _yRopeMax;
            // let the player leave edges when their input aligns
            float sampledPosY = _rb.position.y + _verticalDirection.y;
            bool sampleInBounds = sampledPosY < _yRopeMax && sampledPosY > _yRopeMin;
            
            y = inBounds || sampleInBounds ? _verticalMovement * ropeClimbSpeed : 0;
        }
        
        _rb.linearVelocity = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            _canClimbRope = true;
            
            Rope rope = collision.gameObject.GetComponent<Rope>();
            _yRopeMin = rope.yMin;
            _yRopeMax = rope.yMax;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Rope"))
        {
            _canClimbRope = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_isDead) return;
        
        _moveDirection = context.ReadValue<Vector2>();

        // Performed and canceled callbacks incorrectly flip the transform. Ignore them.
        if (context.performed || context.canceled) return; 
        
        if (Mathf.Sign(_moveDirection.x) != Mathf.Sign(_previousDirection.x))
        {
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDirection.x));
        }
        _previousDirection = _moveDirection;
    }
    
    public void OnMoveVertical(InputAction.CallbackContext context)
    {
        if (_isDead) return;
        
        _verticalDirection = context.ReadValue<Vector2>();
        
        if (_canClimbRope && _verticalDirection.y >= 0.5f)
            _isClimbingRope = true;

        if (!_isClimbingRope && _verticalDirection.y <= -0.5f)
        {
            _cameraManager.ShiftCameraDown();
            _isCrouching = true;
        }
        else
        {
            _cameraManager.ReturnCameraToOriginalPosition();
            _isCrouching = false;
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        _isPressingJump = context.ReadValueAsButton();
        _justPressedJump = context.started;
        if (context.started && _numDoubleJumps > 0) _newJumpPress = true;
    }
    
    public void OnInventoryPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;

        _inputDisabled = !_inputDisabled;
        
        // Disable all actions besides the ability to open/close inventory 
        // so that the player cannot move while it is open
        foreach (InputAction action in _playerInputMap.actions)
        {
            if (action.name != "Inventory")
            {
                if (_inputDisabled) action.Disable();
                else action.Enable();
            }
        }
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
            _numDoubleJumps = passiveSpellAffects.doubleJumps;
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
        if (_currentState == _states.Climb() || _currentState == _states.Rope()) return;
        
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
            Debug.DrawRay(start + (Vector2.up * climbCheckHeight), direction * climbCheckDistance, Color.orange);
            Debug.DrawRay(transform.position, Vector2.down * climbAboveBelowCheckLength, Color.orange);
            Debug.DrawRay(transform.position, Vector3.up * climbAboveBelowCheckLength, Color.orange);
        }

        RaycastHit2D wallToClimb = Physics2D.Raycast(start, direction, climbCheckDistance, environmentLayer);
        _canClimb = !_isGrounded
                    && !_climbCooldown
                    && wallToClimb
                    && !Physics2D.Raycast(start + (Vector2.up * climbCheckHeight), direction, climbCheckDistance, environmentLayer)
                    && !Physics2D.Raycast(transform.position, Vector2.down, climbAboveBelowCheckLength, environmentLayer)
                    && !Physics2D.Raycast(transform.position, Vector2.up, climbAboveBelowCheckLength, environmentLayer);
        
        // Climb state uses the tilemap obtained from this raycast to climb onto
        if (_canClimb && wallToClimb.collider.TryGetComponent<Tilemap>(out Tilemap tilemap))
        {   
            Vector3Int tilePos = tilemap.WorldToCell(start + (direction * climbCheckDistance));
            Vector3 tileCenter = tilemap.GetCellCenterWorld(tilePos);
            Vector3 tileOffset = Vector3.right * (climbDistanceFromWall * -dirSign);
            _climbPosition = tileCenter + tileOffset;
            if (climbDebug) Debug.DrawLine(tileCenter, _climbPosition, Color.red);
        }
        else
        {
            _canClimb = false;
        }
    }

    public void InvokeDoubleJumpComplete()
    {
        OnDoubleJumpComplete?.Invoke();
    }

    public void StartClimbDelay()
    {
        StartCoroutine(ClimbDelay());
    }
    
    private IEnumerator ClimbDelay()
    {
        _climbCooldown = true;
        _climbDelayTimer = 0f;
        while (_climbDelayTimer < climbDelayTime)
        {
            _climbDelayTimer += Time.deltaTime;
            yield return null;
        }
        _climbCooldown = false;
    }
}
