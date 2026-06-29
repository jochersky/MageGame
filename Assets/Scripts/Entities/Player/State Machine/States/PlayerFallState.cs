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
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        Context.HorizontalMovement = Context.MoveDirection.x * Context.Stats.Speed;

        if (Context.IsClimbingRope && Context.VerticalDirection == Vector2.up) SwitchState(Dictionary.Rope());
        else if (Context.NewJumpPress && Context.NumDoubleJumps > 0) SwitchState(Dictionary.Jump());
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
