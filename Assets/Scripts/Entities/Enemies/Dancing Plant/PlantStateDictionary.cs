using System.Collections.Generic;

enum PlantStates
{
    // - Root States -
    Grounded,
    Fall,
    Dead,
    // - Sub States -
    Idle,
    Emerge,

    Hide,
    Walk,
    Aggro,
}

public class PlantStateDictionary
{
    private PlantStateMachine _context;
    private readonly Dictionary<PlantStates, PlantBaseState> _states = new();
    
    public PlantStateDictionary(PlantStateMachine context)
    {
        _context = context;
        
        // Instantiate states for later use instead of reinstantiating later for performance.
        // - Root States -
        _states[PlantStates.Grounded] = new PlantGroundedState(_context, this);
        _states[PlantStates.Fall] = new PlantFallState(_context, this);
        _states[PlantStates.Dead] = new PlantDeadState(_context, this);
        
        // - Sub States -
        _states[PlantStates.Idle] = new PlantIdleState(_context, this);
        _states[PlantStates.Emerge] = new PlantEmergeState(_context, this);
        _states[PlantStates.Hide] = new PlantHideState(_context, this);
        _states[PlantStates.Walk] = new PlantWalkState(_context, this);
        _states[PlantStates.Aggro] = new PlantAggroState(_context, this);
    }
    
    // - Root States -
    public PlantBaseState Grounded()
    {
        return _states[PlantStates.Grounded];
    }
    
    public PlantBaseState Fall()
    {
        return _states[PlantStates.Fall];
    }

    public PlantBaseState Dead()
    {
        return _states[PlantStates.Dead];
    }
    
    // - Sub States -

    public PlantBaseState Idle()
    {
        return _states[PlantStates.Idle];
    }

    public PlantBaseState Emerge()
    {
        return _states[PlantStates.Emerge];
    }

    public PlantBaseState Hide()
    {
        return _states[PlantStates.Hide];
    }
    
    public PlantBaseState Walk()
    {
        return _states[PlantStates.Walk];
    }
    
    public PlantBaseState Aggro()
    {
        return _states[PlantStates.Aggro];
    }
}
