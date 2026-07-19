using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject snakeGO;
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Sensor aggroSensor;
    [SerializeField] private Health health;
    [SerializeField] private Hitbox hitbox;
    [SerializeField] private Hurtbox hurtbox;
    [SerializeField] private Chain chain;
    private Rigidbody2D _rb;

    [Header("Properties")]
    [SerializeField] private float splineFollowSpeed = 5;
    [SerializeField, Range(0, 0.2f)] private float splineProgressSpeed;
    
    // State Variables
    private SnakeBaseState _currentState;
    private SnakeBaseState _currentSubState;
    private SnakeStateDictionary _states;
    
    // Instances Variables
    private bool _playerInRange;
    private bool _dead;
    
    // State Setters & Getters
    public SnakeBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public SnakeBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public SnakeStateDictionary States { get { return _states; } set { _states = value; } }
    public SplineContainer Spline { get { return spline; } set { spline = value; } }
    public GameObject SnakeGO { get { return snakeGO; } set { snakeGO = value; } }
    public Chain Chain { get { return chain; } set { chain = value; } }
    public bool PlayerInRange { get { return _playerInRange; } set { _playerInRange = value; } }
    public Vector3 PlayerPosition => aggroSensor.TargetPosition();
    public bool Dead => _dead;
    public float SplineFollowSpeed { get { return splineFollowSpeed; } set { splineFollowSpeed = value; } }
    public float SplineProgressSpeed { get { return splineProgressSpeed; } set { splineProgressSpeed = value; } }
    
    private void Start()
    {
        aggroSensor.OnTargetChanged += (target) => { _playerInRange = target; };
        
        // State machine initial state setup
        _states = new SnakeStateDictionary(this);
        _currentState = _states.Idle();
        _currentState.EnterState();

        health.OnDeath += () =>
        {
            _dead = true;
            hitbox.gameObject.SetActive(false);
            hurtbox.gameObject.SetActive(false);
        };
    }

    private void Update()
    {
        if (_dead) return;
        
        _currentState.UpdateStates();
    }
}
