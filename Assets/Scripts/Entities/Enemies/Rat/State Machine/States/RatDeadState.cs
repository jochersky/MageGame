using UnityEngine;

public class RatDeadState : RatBaseState
{
    public RatDeadState(RatStateMachine context, RatStateDictionary ratStateDictionary)
        : base(context, ratStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Dead, 0, 0);
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
