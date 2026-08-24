using UnityEngine;

public class PlantEmergeState : PlantBaseState
{
    private CountdownTimer _emergeTimer;
    public PlantEmergeState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
        _emergeTimer = new CountdownTimer(Context.EmergeTime);
    }

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}
