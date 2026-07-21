using UnityEngine;

public class SkeletonAggroState : SkeletonBaseState
{
    public SkeletonAggroState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary)
        : base(currentContext, skeletonStateDictionary)
    {
    }

    public override void EnterState()
    {
        Context.CurrentMoveSpeed = Context.AggroMoveSpeed;
        Context.HorizontalMovement = Context.MoveDir.x * Context.CurrentMoveSpeed;;
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
