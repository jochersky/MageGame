using UnityEngine;

public class RatLungeState : RatBaseState
{
    public RatLungeState(RatStateMachine context, RatStateDictionary ratStateDictionary)
        : base(context, ratStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        Context.Animator.CrossFade(Context.Lunge, 0, 0);
        Context.LinearVelocityX = Context.LinearVelocityX;
        Context.LinearVelocityY = Context.LungeVelocityY;
    }

    public override void UpdateState()
    {
        if (Context.LinearVelocityY < 0) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
