using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }

    public override void EnterState()
    {
        // TODO: change animation to jump animation
        
        PerformJump();
    }

    public override void UpdateState()
    {
        if (Context.LinearVelocityY < 0) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
    
    private void PerformJump()
    {
        
    }
}
