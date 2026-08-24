using UnityEngine;

public class PlantGroundedState : PlantBaseState
{
    public PlantGroundedState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
        IsRootState = true;
    }


    public override void EnterState()
    {
        // since this is a superstate
        InitializeSubState();
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        SetSubState(Dictionary.Idle());
    }

    public override void UpdateState()
    {
        // transitions to other superstates
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (!Context.IsGrounded) SwitchState(Dictionary.Fall());
    }

    public override string ToString() => "PlantGroundedState";
}
