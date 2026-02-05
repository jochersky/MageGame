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
        // TODO: change animation to jump animation
        
        // PerformJump();
        // Debug.Log("Entered Jump State");
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
                // _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, maxJumpHeight);
                Context.LinearVelocityY = Context.MaxJumpHeight;
                Context.WasClimbing = false;
            }
            else
            {
                // _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
                Context.LinearVelocityY = Context.MaxJumpHeight * 0.5f;
            }
        }
    }
}
