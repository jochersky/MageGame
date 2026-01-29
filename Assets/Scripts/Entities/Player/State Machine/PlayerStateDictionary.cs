using UnityEngine;
using System.Collections.Generic;
using TMPro;

enum PlayerStates
{
    // - Root States -
    Grounded,
    Fall,
    // - Sub States -
    Walk
}

public class PlayerStateDictionary
{
    private PlayerStateMachine _context;
    private readonly Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateDictionary(PlayerStateMachine context)
    {
        _context = context;
        
        _states[PlayerStates.Grounded] = new PlayerGroundedState(_context, this);
        _states[PlayerStates.Fall] = new PlayerFallState(_context, this);

        _states[PlayerStates.Walk] = new PlayerWalkState(_context, this);
    }

    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.Grounded];
    }

    public PlayerBaseState Fall()
    {
        return _states[PlayerStates.Fall];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.Walk];
    }
}
