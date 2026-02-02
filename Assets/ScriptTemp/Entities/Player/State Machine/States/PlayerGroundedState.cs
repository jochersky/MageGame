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
        InitializeSubState();
    }

    public override void UpdateState()
    {
        // if (!Context.IsGrounded) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
        
    }
    
    public override void InitializeSubState()
    {
        // switch to idle or walk or run or jump
        if (Context.MoveDirection != Vector2.zero) SetSubState(Dictionary.Walk());
        else if (Context.MoveDirection == Vector2.zero) SetSubState(Dictionary.Idle());
    }
}
