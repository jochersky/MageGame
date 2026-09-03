using UnityEngine;

public class PlayerRopeState : PlayerBaseState
{
    public PlayerRopeState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Jump, 0, 0);
        Context.Rigidbody.gravityScale = 0;
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        Context.VerticalMovement = Context.VerticalDirection.y;
        
        if (Context.IsPressingJump && Context.VerticalDirection.y >= -0.25) SwitchState(Dictionary.Jump());
        else if (!Context.CanClimbRope || (Context.VerticalDirection.y <= -0.25 && Context.NewJumpPress))
        {
            Context.NewJumpPress = false;
            SwitchState(Dictionary.Fall());
        }
    }

    public override void ExitState()
    {
        Context.IsClimbingRope = false;
        Context.WasClimbingRope = true;
    }

    public override void InitializeSubState()
    {
    }
    
    public override string ToString()
    {
        return "PlayerRopeState";
    }
}
