using Unity.VisualScripting;
using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary) 
        : base(context, playerStateDictionary) { }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Idle, 0, 0);
        Context.HorizontalMovement = 0;
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());

        if (!Context.IsCrouching && Context.MoveDirection.x == 0) SwitchState(Dictionary.Idle());
        else if (!Context.IsCrouching && Context.MoveDirection.x != 0) SwitchState(Dictionary.Walk());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
