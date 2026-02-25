using UnityEngine;
using System.Collections.Generic;
using TMPro;

enum PlayerStates
{
    // - Root States -
    Grounded,
    Fall,
    Climb,
    Jump,
    Dead,
    // - Sub States -
    Idle,
    Walk,
}

public class PlayerStateDictionary
{
    private PlayerStateMachine _context;
    private readonly Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateDictionary(PlayerStateMachine context)
    {
        _context = context;
        
        // Instantiate states for later use instead of reinstantiating later for performance.
        // - Root States -
        _states[PlayerStates.Grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.Fall] = new PlayerFallState(_context, this);
        _states[PlayerStates.Climb] = new PlayerClimbState(_context, this);
        _states[PlayerStates.Jump] = new PlayerJumpState(_context, this);
        _states[PlayerStates.Dead] = new PlayerDeadState(_context, this);
        
        // - Sub States -
        _states[PlayerStates.Idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.Walk] = new PlayerWalkState(_context, this);
    }

    // - Root States -
    
    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.Grounded];
    }

    public PlayerBaseState Fall()
    {
        return _states[PlayerStates.Fall];
    }

    public PlayerBaseState Climb()
    {
        return _states[PlayerStates.Climb];
    }
    
    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.Jump];
    }

    public PlayerBaseState Dead()
    {
        return _states[PlayerStates.Dead];
    }
    
    // - Sub States -

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.Idle];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.Walk];
    }
}
