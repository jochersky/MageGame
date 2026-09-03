
public abstract class PlantBaseState
{
    private bool _isRootState = false;
    private PlantStateMachine _context;
    private PlantStateDictionary _dictionary;
    private PlantBaseState _currentSubState;
    private PlantBaseState _currentSuperState;
    
    protected bool IsRootState { set => _isRootState = value; }
    public PlantBaseState SubState { get { return _currentSubState; } private set { _currentSubState = value; } }
    protected PlantStateMachine Context { get { return _context; } set { _context = value; } }
    protected PlantStateDictionary Dictionary { get { return _dictionary; } set { _dictionary = value; } }
    
    // Constructor
    protected PlantBaseState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary)
    {
        _context = currentContext;
        _dictionary = plantStateDictionary;
    }
    
    // First method run after a state is entered
    public abstract void EnterState();

    // Method where state behavior is run. Per frame state transitions checks done here
    public abstract void UpdateState();

    // Last method run after a state is exited
    public abstract void ExitState();

    // For root states that initialize substates
    public abstract void InitializeSubState();
    
    protected void SwitchState(PlantBaseState newState)
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

    protected void SetSuperState(PlantBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlantBaseState newSubState){
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
