using UnityEngine;

public class SnakeAggroState : SnakeBaseState
{
    private bool _loopCompleted;
    private float _t;
    
    public SnakeAggroState(SnakeStateMachine context, SnakeStateDictionary snakeStateDictionary)
        : base(context, snakeStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Spline.transform.position = Context.PlayerPosition;
        _loopCompleted = false;
        Context.Chain.SyncChain();
    }

    public override void UpdateState()
    {
        if (Context.Dead) SwitchState(Dictionary.Dead());
        
        Context.Chain.UpdateChain(Context.SplineProgressSpeed, Context.SplineFollowSpeed);

        if (Context.Chain.Progress >= 0.25 && !_loopCompleted)
        {
            _loopCompleted = true;
            Context.Spline.transform.position = Context.PlayerPosition;
            Context.Chain.SyncChain();
        }
        else if (Context.Chain.Progress < 0.25 && _loopCompleted)
        {
            _loopCompleted = false;
        }
        
        if (!Context.PlayerInRange && _loopCompleted) SwitchState(Dictionary.Idle());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
