using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class SnakeStateMachine : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Sensor aggroSensor;
    [SerializeField] private Health health;
    [SerializeField] private Hitbox[] hitboxes;
    [SerializeField] private Hurtbox[] hurtboxes;
    [SerializeField] private Chain chain;
    private Rigidbody2D _rb;

    [Header("Properties")]
    [SerializeField] private float splineFollowSpeed = 5;
    [SerializeField, Range(0, 0.2f)] private float splineProgressSpeed;
    [SerializeField] private float fallDuration = 8f;
    
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
    public Chain Chain { get { return chain; } set { chain = value; } }
    public bool PlayerInRange { get { return _playerInRange; } set { _playerInRange = value; } }
    public Vector3 PlayerPosition => aggroSensor.TargetPosition();
    public bool Dead => _dead;
    public float SplineFollowSpeed { get { return splineFollowSpeed; } set { splineFollowSpeed = value; } }
    public float SplineProgressSpeed { get { return splineProgressSpeed; } set { splineProgressSpeed = value; } }
    public float FallDuration { get { return fallDuration; } set { fallDuration = value; } }
    
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
            foreach (Hurtbox hurtbox in hurtboxes)
                hurtbox.gameObject.SetActive(false);
            foreach (Hitbox hitbox in hitboxes)
                hitbox.gameObject.SetActive(false);
        };
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }

    public void Destroy()
    {
        // spline is the parent game object
        Destroy(spline.gameObject);
    }
}
