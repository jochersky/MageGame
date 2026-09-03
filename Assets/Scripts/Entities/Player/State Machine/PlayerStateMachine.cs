using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
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
    [SerializeField] private KnockBack knockBack;
    [SerializeField] private StatusEffectManager statusEffectManager;
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
    [SerializeField] private float justJumpedGracePeriod = 0.05f;
    [SerializeField] private float coyoteJumpTimer = 0.1f;
    
    [Header("Dodge")]
    [SerializeField] private float maxDodgeSpeed = 20f;
    [SerializeField] private float dodgeDuration = 0.2f;
    [SerializeField] private float dodgeCooldown = 0.4f;
    [SerializeField] private float dodgeInvulnerabilityTime = 0.2f;
    
    [Header("Airborne")]
    [SerializeField] private float maxAirborneMoveSpeed = 1f;
    
    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2;
    [SerializeField] private float maxFallSpeed = 15;
    [SerializeField] private float fallSpeedMultiplier = 1.5f;

    [Header("Climbing")] 
    [SerializeField, UnityEngine.Range(0.25f, 2)] private float climbDistanceFromWall = 1.25f;
    [SerializeField] private Vector2 climbCheckOffset = Vector2.zero;
    [SerializeField] private Vector2 climbCheckDir = Vector2.right;
    [SerializeField] private float climbCheckDistance = 0.2f;
    [SerializeField] private float climbCheckHeight = 0.7f;
    [SerializeField] private float climbAboveBelowCheckLength = 0.5f;
    [SerializeField] private float climbSnapSpeed = 0.2f;
    [SerializeField] private float climbDelayTime = 0.1f;
    [SerializeField] private bool climbDebug;

    [Header("Rope")] 
    [SerializeField] private RopeHandler _ropeHandler;
    [SerializeField] private float ropeClimbSpeed = 0.25f;

    [Header("Camera Movement")] [SerializeField]
    private float dirHoldDuration = 1.5f;
    
    [Header("Knock Back")]
    [SerializeField, UnityEngine.Range(0, 1)] private float reduceConst = 0.3f;
    
    // State Variables
    private PlayerBaseState _previousState;
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
    private bool _justJumped;
    private float _justJumpedTimer;
    private bool _coyoteJumpDisabled;
    private int _numDoubleJumps;
    private bool _isPressingDodge;
    private bool _canDodge;
    private bool _dodgeInCooldown;
    private bool _dodging;
    private int _numDodges;
    private bool _canClimb;
    private bool _wasClimbing;
    private Vector2 _climbPosition;
    private Vector2 _climbDir;
    private float _climbDelayTimer;
    private bool _climbCooldown;
    // private Tilemap _climbingTilemap;
    private bool _isDead;
    private bool _inputDisabled;
    private bool _canClimbRope;
    private bool _isClimbingRope;
    private bool _wasClimbingRope;
    private float _ropeMidpointX;
    private float _yRopeMin;
    private float _yRopeMax;
    private bool _isCrouching;
    private bool _isLookingUp;
    private Vector2 _knockBackForce;

    private CountdownTimer _lookHoldTimer;

    [Header("State Debug")]
    public String stateName = "";

    // Event for flipping the transform.
    public UnityEvent<float> onDirectionChanged;

    public delegate void DoubleJumpComplete();
    public event DoubleJumpComplete OnDoubleJumpComplete;
    
    // State Setters & Getters
    public PlayerBaseState PreviousState => _previousState;
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
    public bool JustJumped { get { return _justJumped; } set { _justJumped = value; } }
    public int NumDoubleJumps { get { return _numDoubleJumps; } set { _numDoubleJumps = value; } }
    public bool JustPressedJump { get { return _justPressedJump; } set { _justPressedJump = value; } }
    public bool IsPressingJump { get { return _isPressingJump; } set { _isPressingJump = value; } }
    public bool NewJumpPress { get { return _newJumpPress; } set { _newJumpPress = value; } }
    public bool CoyoteJumpDisabled { get { return _coyoteJumpDisabled; } set { _coyoteJumpDisabled = value; } }
    public float MaxDodgeSpeed { get { return maxDodgeSpeed; } set { maxDodgeSpeed = value; } }
    public bool IsPressingDodge { get { return _isPressingDodge; } set { _isPressingDodge = value; } }
    public bool CanDodge { get { return _canDodge; } set { _canDodge = value; } }
    public bool IsDodging { get { return _dodging; } set { _dodging = value; } }
    public int NumDodges { get { return _numDodges; } set { _numDodges = value; } }
    public bool CanClimb { get { return _canClimb; } set { _canClimb = value; } }
    public bool WasClimbing { get { return _wasClimbing; } set { _wasClimbing = value; } }
    public Vector2 ClimbPosition { get { return _climbPosition; } set { _climbPosition = value; } }
    public Vector2 ClimbDir { get { return _climbDir; } set { _climbDir = value; } }
    public float ClimbSnapSpeed { get { return climbSnapSpeed; }  set { climbSnapSpeed = value; } }
    public bool CanClimbRope { get { return _canClimbRope; } set { _canClimbRope = value; } }
    public bool IsClimbingRope { get { return _isClimbingRope; } set { _isClimbingRope = value; } }
    public bool WasClimbingRope { get { return _wasClimbingRope; }  set { _wasClimbingRope = value; } }
    public bool IsCrouching { get { return _isCrouching; } set { _isCrouching = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }
    public bool IsClimbing => _currentState == _states.Climb();

    public int debugCount = 0;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInputMap = playerInput.actions.actionMaps[0];
        _stats = new Stats(new StatsMediator(), baseStats);
        _cameraManager = GetComponentInChildren<CameraManager>();
        _lookHoldTimer = new CountdownTimer(dirHoldDuration);
        
        // Passive spell affects initialization
        _numDoubleJumps = passiveSpellAffects.doubleJumps + baseStats.jumps;
        _numDodges = passiveSpellAffects.dodges + baseStats.dodges;

        health.OnDeath += () => { _isDead = true; };
        knockBack.OnKnockBackApplied += force => { _knockBackForce = force; }; 
        _lookHoldTimer.OnTimerStop += () => { HandleCamera(); };
        
        // State machine initial state setup
        _states = new PlayerStateDictionary(this);
        _currentState = _isGrounded ? _states.Grounded() : _states.Fall();
        _currentState.EnterState();
    
        // provide initial direction for dodging (facing right)
        _previousDirection.x = 1;
        
        statusEffectManager.Initialize(_stats.Mediator);
    }

    private void Update()
    {
        _currentState.UpdateStates();
        _stats.Mediator.Update(Time.deltaTime);
        stateName = _currentState.ToString();
        
        _lookHoldTimer.Tick(Time.deltaTime);
        
        if (_previousState != _currentState) _previousState = _currentState;
    }
    
    void FixedUpdate()
    {
        CheckGrounded();
        CheckClimbing();
        UpdateGravity();

        float x = _horizontalMovement;
        float y = _rb.linearVelocityY;
        
        // lock player onto ladder horizontally until they jump off
        if (_isClimbingRope && _currentState == States.Rope())
        {
            x = 0;
            // keep player in center of the rope
            transform.position = new Vector3(_ropeMidpointX, transform.position.y, transform.position.z);
            
            // player should not be able to "leave" rope by descending or ascending it
            bool inBounds = _rb.position.y > _yRopeMin && _rb.position.y < _yRopeMax;
            if (!inBounds)
            {
                // snap player into place when they are outside the bounds
                float posY = Mathf.Clamp(_rb.position.y, _yRopeMin, _yRopeMax);
                transform.position = new Vector3(_ropeMidpointX, posY + (posY > _yRopeMin ? -0.2f : 0.2f), transform.position.z);
            }
            
            float sampledPosY = _rb.position.y + _verticalDirection.y * 0.2f;
            bool sampleInBounds = sampledPosY < _yRopeMax && sampledPosY > _yRopeMin;
        
            y = sampleInBounds ? _verticalMovement * ropeClimbSpeed : 0;
        }

        _rb.linearVelocity = new Vector2(x, y);
        _rb.linearVelocity += _knockBackForce;

        UpdateKnockBackForce();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_isDead) return;
        
        _moveDirection = context.ReadValue<Vector2>();

        // Performed and canceled callbacks incorrectly flip the transform. Ignore them.
        if (context.performed || context.canceled) return;
        
        if (!IsClimbing) CheckForFlipTransform();

        _previousDirection = _moveDirection;
    }

    public void CheckForFlipTransform()
    {
        bool moveDirChanged = Mathf.Sign(_moveDirection.x) != Mathf.Sign(_previousDirection.x);
        bool changedDirAfterClimbing = Mathf.Sign(_moveDirection.x) != Mathf.Sign(_climbDir.x);
        // Debug.Log($"changedDirAfterClimbing {_moveDirection} {_climbInitialDir}");
        // Debug.Log($"moveDirChanged {_moveDirection} {_previousDirection}");
        if (moveDirChanged || changedDirAfterClimbing)
        {
            onDirectionChanged?.Invoke(Mathf.Sign(_moveDirection.x));
            _climbDir = Vector2.zero;
        }
    }
    
    public void OnMoveVertical(InputAction.CallbackContext context)
    {
        if (_isDead) return;
        
        _verticalDirection = context.ReadValue<Vector2>();

        // rope
        if (_canClimbRope && _verticalDirection.y >= 0.5f)
        {
            _isClimbingRope = true;
        }

        // camera
        if (_verticalDirection.x != 0 || context.canceled || !_isGrounded)
        {
            _lookHoldTimer.Reset();
            _cameraManager.ReturnCameraToOriginalPosition();
            _isCrouching = false;
            _isLookingUp = false;
        }
        else if (context.started && _isGrounded)
        {
            _lookHoldTimer.Start();
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        _isPressingJump = context.ReadValueAsButton();
        _justPressedJump = context.started;
        if (context.started) _newJumpPress = true;
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        _isPressingDodge = context.ReadValueAsButton();
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
        bool rawGrounded = Physics2D.OverlapBox(jumpCheckTransform.position, jumpCheckSize, 0, environmentLayer)
                           && LinearVelocityY <= 0.1f;

        if (_justJumped)
        {
            if (!rawGrounded)
            {
                // actually left the ground
                _justJumped = false;
                _justJumpedTimer = 0f;
            }
            else
            {
                // makes sure it isn't stuck as true forever
                _justJumpedTimer += Time.fixedDeltaTime;
                if (_justJumpedTimer >= justJumpedGracePeriod)
                {
                    _justJumped = false;
                    _justJumpedTimer = 0f;
                }
            }
        }

        bool countsAsGrounded = rawGrounded;

        if (!countsAsGrounded && _canJump)
        {
            _airTime += Time.deltaTime;
            _canJump = _airTime < coyoteJumpTimer && !_coyoteJumpDisabled;
        }
        else if ((countsAsGrounded && !_justJumped) || IsClimbing)
        {
            _airTime = 0;
            _canJump = true;
            _numDoubleJumps = passiveSpellAffects.doubleJumps + baseStats.jumps;
        }

        if (countsAsGrounded && !_dodgeInCooldown)
        {
            _canDodge = true;
            _numDodges = passiveSpellAffects.dodges + baseStats.dodges;
        }

        _isGrounded = countsAsGrounded;
    }
    
    private void OnDrawGizmosSelected()
    {
        // Grounded check gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(jumpCheckTransform.position, jumpCheckSize);
    }

    private void UpdateGravity()
    {
        if (_currentState == _states.Dodge()) return;
        
        if (_currentState == _states.Rope() || IsClimbing || _isGrounded)
        {
            _rb.gravityScale = 0;
            _rb.linearVelocityY = 0f;
        }
        // Player falls down faster with negative y-velocity.
        else if (_currentState == _states.Fall())
        {
            _rb.gravityScale = baseGravity * fallSpeedMultiplier * _stats.GravityFactor;
            _rb.linearVelocityY = Mathf.Max(_rb.linearVelocityY, -maxFallSpeed);
        }
        else
        {
            _rb.gravityScale = baseGravity;
        }
    }

    private void UpdateKnockBackForce()
    {
        _knockBackForce *= reduceConst;
        if (_knockBackForce.magnitude <= 0.1f) _knockBackForce = Vector2.zero;
    }

    public void EnemyStomped()
    {
        _rb.linearVelocityY = maxJumpHeight;
    }

    private void CheckClimbing()
    {
        // rope
        Rope ropeToClimb = _ropeHandler.GetRope();
        if (ropeToClimb)
        {
            _canClimbRope = true;
            _yRopeMax = ropeToClimb.yMax;
            _yRopeMin = ropeToClimb.yMin;
            _ropeMidpointX = ropeToClimb.transform.position.x;
        }
        else
        {
            _canClimbRope = false;
        }
        
        // ledge
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
    
    private void HandleCamera()
    {
        if (!_isClimbingRope && _verticalDirection.y <= -0.5f)
        {
            _cameraManager.ShiftCameraDown();
            _isCrouching = true;
        }
        else if (!_isClimbingRope && _verticalDirection.y > 0.5f)
        {
            _cameraManager.ShiftCameraUp();
            _isLookingUp = true;
        }
    }

    public void InvokeDoubleJumpComplete()
    {
        OnDoubleJumpComplete?.Invoke();
    }

    public void StartDodgeAndCooldown()
    {
        StartCoroutine(DodgeAndCooldown());
    }

    private IEnumerator DodgeAndCooldown()
    {
        _canDodge = false;
        _dodgeInCooldown = true;
        _dodging = true;
        
        // apply dodge movement
        _horizontalMovement = _previousDirection.x * maxDodgeSpeed;
        
        yield return new WaitForSeconds(dodgeDuration);

        // revert dodge movement
        _horizontalMovement = 0f;
        _dodging = false;
        
        yield return new WaitForSeconds(dodgeCooldown);
        
        _dodgeInCooldown = false;
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
