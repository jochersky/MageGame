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
        
        Context.HorizontalMovement = Context.MoveDirection.x * Context.MaxWalkSpeed;
        
        if (Context.MoveDirection == Vector2.zero) SwitchState(Dictionary.Idle());
    }

    public override void ExitState()
    {
    }
    
    public override void InitializeSubState()
    {
    }
}
