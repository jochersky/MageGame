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
        Context.WasClimbing = true;
        // Debug.Log("Entering Climb State");
    }

    public override void UpdateState()
    {
        if (Context.IsPressingJump) SwitchState(Dictionary.Jump());
        else if (Context.MoveDirection.y < 0)
        {
            Context.WasClimbing = false;
            SwitchState(Dictionary.Fall());
        }
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
