using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary) { }

    public override void EnterState()
    {
        Context.HorizontalMovement = 0;
    }

    public override void UpdateState()
    {
        if (Context.MoveDirection != Vector2.zero) SwitchState(Dictionary.Walk());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
