using UnityEngine;

public abstract class RatBaseState
{
    private bool _isRootState = false;
    private RatStateMachine _context;
    private RatStateDictionary _dictionary;
    private RatBaseState _currentSubState;
    private RatBaseState _currentSuperState;
    
    protected bool IsRootState { set => _isRootState = value; }
    protected RatStateMachine Context { get { return _context; } set { _context = value; } }
    protected RatStateDictionary Dictionary { get { return _dictionary; } set { _dictionary = value; } }
    
    // Constructor
    protected RatBaseState(RatStateMachine currentContext, RatStateDictionary ratStateDictionary)
    {
        _context = currentContext;
        _dictionary = ratStateDictionary;
    }
    
    // First method run after a state is entered
    public abstract void EnterState();

    // Method where state behavior is run. Per frame state transitions checks done here
    public abstract void UpdateState();

    // Last method run after a state is exited
    public abstract void ExitState();

    // For root states that initialize substates
    public abstract void InitializeSubState();
    
    protected void SwitchState(RatBaseState newState)
    {
        ExitState();
        newState.EnterState();

        if (_isRootState)
        {
            _context.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
            _context.CurrentSubState = newState;
        }
    }

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState == null) return;
        _currentSubState.UpdateStates();
    }

    protected void SetSuperState(RatBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(RatBaseState newSubState){
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
