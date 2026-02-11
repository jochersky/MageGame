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
    }

    public override void UpdateState()
    {
        PerformJump();
        Context.HorizontalMovement = Context.MoveDirection.x * Context.MaxAirborneMoveSpeed;
        
        if (Context.LinearVelocityY < 0 || !Context.IsPressingJump) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
    
    private void PerformJump()
    {
        if (Context.CanJump || Context.WasClimbing)
        {
            if (Context.IsPressingJump)
            {
                Context.LinearVelocityY = Context.MaxJumpHeight;
                // Toggle for when climbing and trying to jump since CanJump is false when climbing.
                Context.WasClimbing = false;
            }
            else
            {
                Context.LinearVelocityY = Context.MaxJumpHeight * 0.5f;
            }
        }
    }
    
    public override string ToString()
    {
        return "PlayerJumpState";
    }
}
