using UnityEngine;

public class PlantDeadState : PlantBaseState
{
    public PlantDeadState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Dead, 0);
        
        Context.HorizontalMovement = 0;
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        
    }

    public override string ToString() => "PlantDeadState";
}
