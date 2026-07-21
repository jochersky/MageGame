using UnityEngine;

public class SkeletonGroundedState : SkeletonBaseState
{
    
    public SkeletonGroundedState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary) 
        : base(currentContext, skeletonStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (!Context.IsGrounded) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        SetSubState(Dictionary.Idle());
    }
    
    public override string ToString() => "SkeletonGroundedState";
}
