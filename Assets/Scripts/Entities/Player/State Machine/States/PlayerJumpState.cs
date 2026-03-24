using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Jump, 0, 0);
        PerformJump();
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        Context.HorizontalMovement = Context.MoveDirection.x * Context.MaxAirborneMoveSpeed;
        
        if (Context.LinearVelocityY < -1 || !Context.IsPressingJump) SwitchState(Dictionary.Fall());
        else if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override void ExitState()
    {
        Context.NewJumpPress = false;
    }

    public override void InitializeSubState()
    {
    }
    
    private void PerformJump()
    {
        // first jump uses CanJump or WasClimbing
        if ((Context.CanJump || Context.WasClimbing) && Context.IsPressingJump)
        {
            Context.LinearVelocityY = Context.MaxJumpHeight;
            // Toggle for when climbing and trying to jump since CanJump is false when climbing.
            Context.WasClimbing = false;
        }
        // subsequent jumps look at NumDoubleJumps to jump in the air
        else if (Context.NewJumpPress && Context.NumDoubleJumps > 0)
        {
            Context.LinearVelocityY = Context.MaxDoubleJumpHeight;
            Context.NumDoubleJumps--;
            Context.InvokeDoubleJumpComplete();
        }
    }
    
    public override string ToString()
    {
        return "PlayerJumpState";
    }
}
