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
        Context.HorizontalMovement = Context.MoveDirection.x * Context.MaxAirborneMoveSpeed;
        
        if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
        else if (Context.CanClimb && Context.MoveDirection.x != 0) SwitchState(Dictionary.Climb());
    }

    public override void ExitState()
    {
        
    }
    
    public override void InitializeSubState()
    {
    }
    
    public override string ToString()
    {
        return "PlayerFallState";
    }
}
