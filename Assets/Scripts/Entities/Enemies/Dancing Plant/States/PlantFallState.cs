using UnityEngine;

public class PlantFallState : PlantBaseState
{
    public PlantFallState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
    }

    public override void EnterState()
    {
        IsRootState = true;
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
       
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (Context.IsGrounded) SwitchState(Dictionary.Grounded());
    }

    public override string ToString() => "PlantFallState";
}
