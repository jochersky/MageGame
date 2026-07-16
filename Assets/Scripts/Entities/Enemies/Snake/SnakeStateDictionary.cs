using System.Collections.Generic;
using UnityEngine;

enum SnakeStates
{
    Idle,
    Aggro,
    Dead
}

public class SnakeStateDictionary
{
    private SnakeStateMachine _context;
    private readonly Dictionary<SnakeStates, SnakeBaseState> _states = new();

    public SnakeStateDictionary(SnakeStateMachine context)
    {
        _context = context;
        
        // Instantiate states for later use instead of reinstantiating later for performance.
        _states[SnakeStates.Idle] = new SnakeIdleState(context, this);
        _states[SnakeStates.Aggro] = new SnakeAggroState(context, this);
        _states[SnakeStates.Dead] = new SnakeDeadState(context, this);
    }

    public SnakeBaseState Idle()
    {
        return _states[SnakeStates.Idle];
    }

    public SnakeBaseState Aggro()
    {
        return _states[SnakeStates.Aggro];
    }

    public SnakeBaseState Dead()
    {
        return _states[SnakeStates.Dead];
    }
}
