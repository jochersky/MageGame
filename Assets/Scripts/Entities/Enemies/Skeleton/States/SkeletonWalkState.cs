using UnityEngine;

public class SkeletonWalkState : SkeletonBaseState
{
    private CountdownTimer _walkTimer;
    
    public SkeletonWalkState(SkeletonStateMachine currentContext, SkeletonStateDictionary skeletonStateDictionary) 
        : base(currentContext, skeletonStateDictionary)
    {
        _walkTimer = new CountdownTimer(Context.WalkTime);
    }

    public override void EnterState()
    {
        if (_walkTimer.Time == 0) _walkTimer.Time = Context.WalkTime;
        _walkTimer.Start();
        
        Context.Animator.CrossFade(Context.Walk, 0);

        Context.CurrentMoveSpeed = Context.DefaultMoveSpeed;
        Context.HorizontalMovement = Context.MoveDir.x * Context.CurrentMoveSpeed;
    }

    public override void UpdateState()
    {
        if (Context.IsAggroed) SwitchState(Dictionary.Aggro());
        
        _walkTimer.Tick(Time.deltaTime);
        if (_walkTimer.IsFinished) SwitchState(Dictionary.Idle());
    }

    public override void ExitState()
    {
        _walkTimer.Stop();
    }

    public override void InitializeSubState()
    {
    }
    
    public override string ToString() => "SkeletonWalkState";
}
