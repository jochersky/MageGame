using UnityEngine;

public class RatStateMachine : MonoBehaviour
{
    // State Variables
    private RatBaseState _currentState;
    private RatBaseState _currentSubState;
    private RatStateDictionary _states;

    // State Setters & Getters
    public RatBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public RatBaseState CurrentSubState { get { return _currentSubState; } set { _currentSubState = value; } }
    public RatStateDictionary States { get { return _states; } set { _states = value; } }
    
    private void Awake()
    {
        // State machine initial state setup
        _states = new RatStateDictionary(this);
        _currentState.EnterState();
    }

    private void Update()
    {
        _currentState.UpdateStates();
    }
}
