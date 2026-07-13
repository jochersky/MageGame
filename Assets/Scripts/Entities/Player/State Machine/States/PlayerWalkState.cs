using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }
    
    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Walk, 0, 0);
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        // this is so stupid. HorizontalMovement is set to this value while walking and dodge is peformed.
        if (!Context.IsDodging) Context.HorizontalMovement = Context.MoveDirection.x * Context.Stats.Speed;
        
        if (Context.MoveDirection == Vector2.zero) SwitchState(Dictionary.Idle());
        else if (Context.IsPressingDodge && Context.NumDodges > 0 && Context.CanDodge) SwitchState(Dictionary.Dodge());
        else if (Context.IsClimbingRope && Context.VerticalDirection == Vector2.up) SwitchState(Dictionary.Rope());
        else if (Context.IsCrouching) SwitchState(Dictionary.Crouch());
    }

    public override void ExitState()
    {
    }
    
    public override void InitializeSubState()
    {
    }
}
