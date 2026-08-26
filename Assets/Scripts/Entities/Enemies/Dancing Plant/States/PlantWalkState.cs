using UnityEngine;

public class PlantWalkState : PlantBaseState
{
    public PlantWalkState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Walk, 0);

        Context.CurrentMoveSpeed = Context.DefaultMoveSpeed;
        Context.HorizontalMovement = Context.MoveDir.x * Context.CurrentMoveSpeed;
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        if (Context.IsAggroed || Context.TookDamage) SwitchState(Dictionary.Aggro());
    }
}
