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
        
        Context.HorizontalMovement = Context.MoveDirection.x * Context.Stats.Speed;
        
        // allows double jumps to occur while in the Jump state
        TryDoubleJump();
        
        if (Context.IsClimbingRope && Context.VerticalDirection == Vector2.up) SwitchState(Dictionary.Rope());
        else if (Context.IsPressingDodge && Context.NumDodges > 0 && Context.CanDodge) SwitchState(Dictionary.Dodge());
        else if (Context.LinearVelocityY < 0)
        {
            Context.CoyoteJumpDisabled = true;
            SwitchState(Dictionary.Fall());
        }
        else if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
        else if (Context.CanClimb && Context.MoveDirection.x != 0) SwitchState(Dictionary.Climb());
    }

    public override void ExitState()
    {
        Context.NewJumpPress = false;
        Context.WasClimbingRope = false;
    }

    public override void InitializeSubState()
    {
    }
    
    private void PerformJump()
    {
        // first jump uses CanJump or WasClimbing
        if ((Context.CanJump || Context.WasClimbing || Context.WasClimbingRope) && Context.NewJumpPress)
        {
            Context.LinearVelocityY = Context.MaxJumpHeight;
            Context.CoyoteJumpDisabled = true;
            Context.JustJumped = true;
            Context.NewJumpPress = false;
            // Toggle for when climbing and trying to jump since CanJump is false when climbing.
            Context.WasClimbing = false;
        }
        else
        {
            TryDoubleJump();
        }
    }
    
    private void TryDoubleJump()
    {
        if (!Context.NewJumpPress || Context.NumDoubleJumps <= 0) return;
        Context.LinearVelocityY = Context.MaxDoubleJumpHeight;
        Context.NumDoubleJumps--;
        Context.NewJumpPress = false; // consume immediately so this can't re-fire next frame
        Context.InvokeDoubleJumpComplete();
    }
    
    public override string ToString()
    {
        return "PlayerJumpState";
    }
}
