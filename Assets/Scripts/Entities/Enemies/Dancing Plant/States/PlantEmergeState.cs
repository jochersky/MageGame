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
        if (_emergeTimer.Time == 0) _emergeTimer.Time = Context.EmergeTime;
        _emergeTimer.Start();
        Context.Animator.CrossFade(Context.Emerge, 0);
        Context.CurrentMoveSpeed = 0;
        Context.HorizontalMovement = Context.MoveDir.x * Context.CurrentMoveSpeed;
    }

    public override void ExitState()
    {
        _emergeTimer.Stop();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        _emergeTimer.Tick(Time.deltaTime);
        if (_emergeTimer.IsFinished) SwitchState(Dictionary.Walk());
    }

    public override string ToString() => "PlantEmergeState";
}
