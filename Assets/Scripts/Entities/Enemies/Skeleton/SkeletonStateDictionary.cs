using System.Collections.Generic;
using UnityEngine;

enum SkeletonStates
{
    // - Root States -
    Grounded,
    Fall,
    Dead,
    // - Sub States -
    Idle,
    Walk,
    Aggro,
}

public class SkeletonStateDictionary
{
    private SkeletonStateMachine _context;
    private readonly Dictionary<SkeletonStates, SkeletonBaseState> _states = new();
    
    public SkeletonStateDictionary(SkeletonStateMachine context)
    {
        _context = context;
        
        // Instantiate states for later use instead of reinstantiating later for performance.
        // - Root States -
        _states[SkeletonStates.Grounded] = new SkeletonGroundedState(_context, this);
        _states[SkeletonStates.Fall] = new SkeletonFallState(_context, this);
        _states[SkeletonStates.Dead] = new SkeletonDeadState(_context, this);
        
        // - Sub States -
        _states[SkeletonStates.Idle] = new SkeletonIdleState(_context, this);
        _states[SkeletonStates.Walk] = new SkeletonWalkState(_context, this);
        _states[SkeletonStates.Aggro] = new SkeletonAggroState(_context, this);
    }
    
    // - Root States -
    public SkeletonBaseState Grounded()
    {
        return _states[SkeletonStates.Grounded];
    }
    
    public SkeletonBaseState Fall()
    {
        return _states[SkeletonStates.Fall];
    }

    public SkeletonBaseState Dead()
    {
        return _states[SkeletonStates.Dead];
    }
    
    // - Sub States -

    public SkeletonBaseState Idle()
    {
        return _states[SkeletonStates.Idle];
    }
    
    public SkeletonBaseState Walk()
    {
        return _states[SkeletonStates.Walk];
    }
    
    public SkeletonBaseState Aggro()
    {
        return _states[SkeletonStates.Aggro];
    }
}
