using UnityEngine;
using System.Collections.Generic;
using TMPro;

enum PlayerStates
{
    // - Root States -
    Grounded,
    Fall,
    Climb,
    // - Sub States -
    Idle,
    Walk,
    Jump,
}

public class PlayerStateDictionary
{
    private PlayerStateMachine _context;
    private readonly Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateDictionary(PlayerStateMachine context)
    {
        _context = context;
        
        // Instantiate states for later use instead of regenerating later for performance
        // - Root States -
        _states[PlayerStates.Grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.Fall] = new PlayerFallState(_context, this);
        _states[PlayerStates.Climb] = new PlayerClimbState(_context, this);
        
        // - Sub States -
        _states[PlayerStates.Idle] = new PlayerIdleState(_context, this);
        _states[PlayerStates.Walk] = new PlayerWalkState(_context, this);
        _states[PlayerStates.Jump] = new PlayerJumpState(_context, this);
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
    
    // - Sub States -

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.Idle];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.Walk];
    }

    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.Jump];
    }
}
