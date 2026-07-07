using UnityEngine;

public class PlayerDodgeState : PlayerBaseState
{
    public PlayerDodgeState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }
    
    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Jump, 0, 0);
        Context.StartDodgeAndCooldown();
    }

    public override void UpdateState()
    { 
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (Context.IsDodging) return;
        
        if (Context.NewJumpPress && Context.NumDoubleJumps > 0) SwitchState(Dictionary.Jump());
        else if (Context.LinearVelocityY < -1 || !Context.IsPressingJump) SwitchState(Dictionary.Fall());
        else if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override void ExitState()
    {
    }
    
    public override void InitializeSubState()
    {
    }
    
    public override string ToString()
    {
        return "PlayerDodgeState";
    }
}
