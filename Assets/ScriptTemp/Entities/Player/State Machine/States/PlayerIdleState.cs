using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }

    public override void EnterState()
    {
        // TODO: change animation to idle animation
        
        // Player should be still when in idle state
        Context.HorizontalMovement = 0;
    }

    public override void UpdateState()
    {
        if (Context.MoveDirection != Vector2.zero) SwitchState(Dictionary.Walk());
        else if (Context.CanJump && Context.IsPressingJump) SwitchState(Dictionary.Jump());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
