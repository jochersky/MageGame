using UnityEngine;

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {
        if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override void ExitState()
    {
        
    }
    
    public override void InitializeSubState()
    {
        // switch to an attacking or climbing state
    }
}
