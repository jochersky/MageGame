using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    public PlayerClimbState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Rigidbody.gravityScale = 0;
        Context.LinearVelocityY = 0;
    }

    public override void UpdateState()
    {
        // SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
