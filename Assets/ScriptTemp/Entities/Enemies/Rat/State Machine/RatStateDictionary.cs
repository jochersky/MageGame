using System.Collections.Generic;
using UnityEngine;

enum RatStates
{
    // - Root States -
    Grounded,
    Fall,
    // - Sub States -
    Idle,
    Patrolling,
    Chase
}

public class RatStateDictionary
{
    private RatStateMachine _context;
    private readonly Dictionary<RatStates, RatBaseState> _states = new Dictionary<RatStates, RatBaseState>();
    
    public RatStateDictionary(RatStateMachine context)
    {
        _context = context;
    }
}
