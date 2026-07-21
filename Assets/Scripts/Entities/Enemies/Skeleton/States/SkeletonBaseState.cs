using UnityEngine;

public abstract class SkeletonBaseState
{
    private bool _isRootState = false;
    private SkeletonStateMachine _context;
    private SkeletonStateDictionary _dictionary;
    private SkeletonBaseState _currentSubState;
    private SkeletonBaseState _currentSuperState;
    
    protected bool IsRootState { set => _isRootState = value; }
    public SkeletonBaseState SubState { get { return _currentSubState; } private set { _currentSubState = value; } }
    protected SkeletonStateMachine Context { get { return _context; } set { _context = value; } }
    protected SkeletonStateDictionary Dictionary { get { return _dictionary; } set { _dictionary = value; } }
    
    // Constructor
    protected SkeletonBaseState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary)
    {
        _context = currentContext;
        _dictionary = skeletonStateDictionary;
    }
    
    // First method run after a state is entered
    public abstract void EnterState();

    // Method where state behavior is run. Per frame state transitions checks done here
    public abstract void UpdateState();

    // Last method run after a state is exited
    public abstract void ExitState();

    // For root states that initialize substates
    public abstract void InitializeSubState();
    
    protected void SwitchState(SkeletonBaseState newState)
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

    protected void SetSuperState(SkeletonBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(SkeletonBaseState newSubState){
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
