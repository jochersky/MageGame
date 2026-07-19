using UnityEngine;

public class SnakeDeadState : SnakeBaseState
{
    public SnakeDeadState(SnakeStateMachine currentContext, SnakeStateDictionary ratStateDictionary) 
        : base(currentContext, ratStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
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
