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
        // UpdateGravity();
        Context.HorizontalMovement = Context.MoveDirection.x * Context.MaxAirborneMoveSpeed;
        
        if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override void ExitState()
    {
        
    }
    
    public override void InitializeSubState()
    {
        // switch to an attacking or climbing state
    }
    
    private void UpdateGravity()
    {
        if (Context.LinearVelocityY < 0)
        {
            Context.GravityScale = Context.BaseGravity * Context.FallSpeedMultiplier;
            Context.VerticalMovement = Mathf.Max(Context.LinearVelocityY, -Context.MaxFallSpeed);
        }
        else
        {
            Context.GravityScale = Context.BaseGravity;
        }
    }
}
