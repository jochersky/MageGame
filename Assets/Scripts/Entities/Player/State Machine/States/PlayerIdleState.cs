using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Idle, 0, 0);
        Context.HorizontalMovement = 0;
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (Context.MoveDirection.x != 0) SwitchState(Dictionary.Walk());
        else if (Context.IsClimbingRope && Context.VerticalDirection == Vector2.up) SwitchState(Dictionary.Rope());
        else if (Context.IsCrouching) SwitchState(Dictionary.Crouch());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
    
    public override string ToString()
    {
        return "PlayerIdleState";
    }
}
