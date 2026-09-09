using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Context.HorizontalMovement = 0;
        Context.VerticalMovement = 0;
        Context.CoyoteJumpDisabled = false;
        InitializeSubState();
    }

    public override void UpdateState()
    {
        
        if (Context.CanJump && Context.NewJumpPress)
        {
            Context.CoyoteJumpDisabled = true;
            SwitchState(Dictionary.Jump());
        }
        else if (Context.IsPressingDodge && Context.NumDodges > 0 && Context.CanDodge) SwitchState(Dictionary.Dodge());
        else if (!Context.IsGrounded && Context.LinearVelocityY < -0.1f) SwitchState(Dictionary.Fall());
        else if (Context.IsClimbingRope && Context.VerticalDirection == Vector2.up) SwitchState(Dictionary.Rope());
    }

    public override void ExitState()
    {
        
    }
    
    public override void InitializeSubState()
    {
        if (!Mathf.Approximately(Context.MoveDirection.x, 0f)) SetSubState(Dictionary.Walk());
        else if (Mathf.Approximately(Context.MoveDirection.x, 0f)) SetSubState(Dictionary.Idle());
    }
    
    public override string ToString()
    {
        return "PlayerGroundedState";
    }
}
