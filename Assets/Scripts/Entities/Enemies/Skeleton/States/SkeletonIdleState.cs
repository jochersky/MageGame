using UnityEngine;

public class SkeletonIdleState : SkeletonBaseState
{
    private CountdownTimer _idleTimer;
    
    public SkeletonIdleState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary) 
        : base(currentContext, skeletonStateDictionary)
    {
        _idleTimer = new CountdownTimer(Context.IdleTime);
    }

    public override void EnterState()
    {
        if (_idleTimer.Time == 0) _idleTimer.Time = Context.IdleTime;
        _idleTimer.Start();
        
        // call animator
        Context.CurrentMoveSpeed = 0;
        Context.HorizontalMovement = Context.MoveDir.x * Context.CurrentMoveSpeed;
    }

    public override void UpdateState()
    {
        if (Context.IsAggroed) SwitchState(Dictionary.Aggro());
        
        _idleTimer.Tick(Time.deltaTime);
        if (_idleTimer.IsFinished) SwitchState(Dictionary.Walk());
    }

    public override void ExitState()
    {
        _idleTimer.Stop();
    }

    public override void InitializeSubState()
    {
    }
    
    public override string ToString() => "SkeletonIdleState";
}
