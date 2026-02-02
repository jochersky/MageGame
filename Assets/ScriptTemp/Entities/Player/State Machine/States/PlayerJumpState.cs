using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }

    public override void EnterState()
    {
        // TODO: change animation to jump animation
        
        // PerformJump();
    }

    public override void UpdateState()
    {
        Context.HorizontalMovement = Context.MoveDirection.x * Context.MaxAirborneMoveSpeed;
        PerformJump();
        
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
        if (Context.CanJump)
        {
            if (Context.IsPressingJump)
            {
                // _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, maxJumpHeight);
                Context.LinearVelocityY = Context.MaxJumpHeight;
            }
            else
            {
                // _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.5f);
                Context.LinearVelocityY = Context.MaxJumpHeight * 0.5f;
            }
        }
    }
}
