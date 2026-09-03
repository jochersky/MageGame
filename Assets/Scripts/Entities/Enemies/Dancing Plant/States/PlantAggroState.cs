using UnityEngine;
public class PlantAggroState : PlantBaseState
{
    private CountdownTimer _aggroTimer;
    public PlantAggroState(PlantStateMachine currentContext, PlantStateDictionary plantStateDictionary) : base(currentContext, plantStateDictionary)
    {
        _aggroTimer = new CountdownTimer(Context.AggroTime);
    }

    public override void EnterState()
    {
        if (_aggroTimer.Time == 0) _aggroTimer.Time = Context.AggroTime;
        _aggroTimer.Start();
        Context.Animator.CrossFade(Context.Aggro, 0);
        
        Context.CurrentMoveSpeed = Context.AggroMoveSpeed;
        Context.HorizontalMovement = Context.MoveDir.x * Context.CurrentMoveSpeed;;
    }

    public override void ExitState()
    {
        _aggroTimer.Stop();
    }

    public override void InitializeSubState()
    {
    }

    public override void UpdateState()
    {
        if (_aggroTimer.IsFinished)
        {
            Context.IsAggroed = false;
            Context.CurrentHealth = Context.MaxHealth;
            SwitchState(Dictionary.Hide()); 
        } 
        _aggroTimer.Tick(Time.deltaTime);
        // if (_aggroTimer.IsFinished && !Context.IsAggroed)
        // {
        //     SwitchState(Dictionary.Hide());
        // } else
        // {
        //     _aggroTimer.Time = Context.AggroTime;
        // }
    }
}
