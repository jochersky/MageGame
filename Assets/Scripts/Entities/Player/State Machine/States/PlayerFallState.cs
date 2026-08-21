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
        Context.Animator.CrossFade(Context.Fall, 0, 0);
        Context.IsClimbingRope = false;
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        Context.HorizontalMovement = Context.MoveDirection.x * Context.Stats.Speed;

        if (Context.IsClimbingRope && Context.VerticalDirection == Vector2.up) SwitchState(Dictionary.Rope());
        else if (Context.IsPressingJump && (Context.CanJump || Context.NumDoubleJumps > 0))
        {
            Debug.Log($"{Context.NewJumpPress} && ({Context.CanJump} || {Context.NumDoubleJumps > 0})");
            Context.CoyoteJumpDisabled = true;
            SwitchState(Dictionary.Jump());
        }
        else if (Context.IsPressingDodge && Context.NumDodges > 0 && Context.CanDodge) SwitchState(Dictionary.Dodge());
        else if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
        else if (Context.CanClimb && Context.MoveDirection.x != 0) SwitchState(Dictionary.Climb());
    }

    public override void ExitState()
    {
        Context.WasClimbingRope = false;
    }
    
    public override void InitializeSubState()
    {
    }
    
    public override string ToString()
    {
        return "PlayerFallState";
    }
}
