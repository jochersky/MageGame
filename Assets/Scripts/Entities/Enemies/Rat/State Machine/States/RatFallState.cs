using UnityEditor;
using UnityEngine;

public class RatFallState : RatBaseState
{
    public RatFallState(RatStateMachine context, RatStateDictionary ratStateDictionary)
        : base(context, ratStateDictionary)
    {
        IsRootState = true;
    }


    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Fall, 0, 0);
    }

    public override void UpdateState()
    {
        if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
