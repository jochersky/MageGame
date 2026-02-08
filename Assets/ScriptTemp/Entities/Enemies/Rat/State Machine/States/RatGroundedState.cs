using UnityEngine;

public class RatGroundedState : RatBaseState
{
    public RatGroundedState(RatStateMachine context, RatStateDictionary ratStateDictionary)
        : base(context, ratStateDictionary)
    {
        IsRootState = true;
    }


    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
        if (!Context.IsGrounded) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        SetSubState(Dictionary.Patrol());
    }
}
