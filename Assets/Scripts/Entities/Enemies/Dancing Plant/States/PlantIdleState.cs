using UnityEngine;

public class PlantIdleState : PlantBaseState
{
    public PlantIdleState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Idle, 0);
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        if (Context.IsAggroed || Context.TookDamage) SwitchState(Dictionary.Emerge());
    }

    public override string ToString() => "PlantIdleState";

    
}
