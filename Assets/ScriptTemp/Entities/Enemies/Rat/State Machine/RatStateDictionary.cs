using System.Collections.Generic;
using UnityEngine;

enum RatStates
{
    // - Root States -
    Grounded,
    Fall,
    // - Sub States -
    Patrol,
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
        
        // - Sub States -
        _states[RatStates.Patrol] = new RatPatrolState(_context, this);
    }

    public RatBaseState Grounded()
    {
        return _states[RatStates.Grounded];
    }
    
    public RatBaseState Fall()
    {
        return _states[RatStates.Fall];
    }

    public RatBaseState Patrol()
    {
        return _states[RatStates.Patrol];
    }
}
