using UnityEngine;

public abstract class SnakeBaseState
{
    private bool _isRootState = false;
    private SnakeStateMachine _context;
    private SnakeStateDictionary _dictionary;
    private SnakeBaseState _currentSubState;
    private SnakeBaseState _currentSuperState;
    
    protected bool IsRootState { set => _isRootState = value; }
    protected SnakeStateMachine Context { get { return _context; } set { _context = value; } }
    protected SnakeStateDictionary Dictionary { get { return _dictionary; } set { _dictionary = value; } }
    
    // Constructor
    protected SnakeBaseState(SnakeStateMachine currentContext, SnakeStateDictionary ratStateDictionary)
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
    
    protected void SwitchState(SnakeBaseState newState)
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

    protected void SetSuperState(SnakeBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(SnakeBaseState newSubState){
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
