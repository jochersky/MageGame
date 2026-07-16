using UnityEngine;

public class SnakeIdleState : SnakeBaseState
{
    public SnakeIdleState(SnakeStateMachine context, SnakeStateDictionary snakeStateDictionary)
        : base(context, snakeStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
    }

    public override void UpdateState()
    {
        if (Context.Dead) SwitchState(Dictionary.Dead());

        if (Context.PlayerInRange) SwitchState(Dictionary.Aggro());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
