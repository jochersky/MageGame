using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }
    
    public override void EnterState()
    {
        // TODO: change animation to walk animation
    }

    public override void UpdateState()
    {
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
