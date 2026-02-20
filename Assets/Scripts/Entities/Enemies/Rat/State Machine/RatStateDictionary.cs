using System.Collections.Generic;
using UnityEngine;

enum RatStates
{
    // - Root States -
    Grounded,
    Fall,
    Lunge,
    Dead
    // - Sub States -
}

public class RatStateDictionary
{
    private RatStateMachine _context;
    private readonly Dictionary<RatStates, RatBaseState> _states = new Dictionary<RatStates, RatBaseState>();
    
    public RatStateDictionary(RatStateMachine context)
    {
        _context = context;
        
        // Instantiate states for later use instead of reinstantiating later for performance.
        // - Root States -
        _states[RatStates.Grounded] = new RatGroundedState(_context, this);
        _states[RatStates.Fall] = new RatFallState(_context, this);
        _states[RatStates.Lunge] = new RatLungeState(_context, this);
        _states[RatStates.Dead] = new RatDeadState(_context, this);

        // - Sub States -
    }
    
    // - Root States -
    public RatBaseState Grounded()
    {
        return _states[RatStates.Grounded];
    }
    
    public RatBaseState Fall()
    {
        return _states[RatStates.Fall];
    }

    public RatBaseState Lunge()
    {
        return _states[RatStates.Lunge];
    }

    public RatBaseState Dead()
    {
        return _states[RatStates.Dead];
    }
    
    // - Sub States -
}
