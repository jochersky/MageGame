using UnityEngine;

public class SkeletonFallState : SkeletonBaseState
{
    public SkeletonFallState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary) 
        : base(currentContext, skeletonStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Fall, 0);
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }

    public override string ToString() => "SkeletonFallState";
}
