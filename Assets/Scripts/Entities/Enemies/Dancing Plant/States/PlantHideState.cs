using UnityEngine;

public class PlantHideState : PlantBaseState
{
    private CountdownTimer _emergeTimer;
    public PlantHideState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
        _emergeTimer = new CountdownTimer(Context.EmergeTime);
    }

    public override void EnterState()
    {
        if (_emergeTimer.Time == 0) _emergeTimer.Time = Context.EmergeTime;
        _emergeTimer.Start();
        Context.Animator.CrossFade(Context.Hide, 0);
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
        if (_emergeTimer.IsFinished) SwitchState(Dictionary.Idle());
    }

    public override string ToString() => "PlantHideState";
}
