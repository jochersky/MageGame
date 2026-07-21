using UnityEngine;

public class SkeletonDeadState : SkeletonBaseState
{
    public SkeletonDeadState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary) 
        : base(currentContext, skeletonStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Dead, 0);
        
        Context.HorizontalMovement = 0;
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
