using UnityEngine;

public class RatGroundedState : RatBaseState
{
    private float _lungeTimer = 0f;
    
    public RatGroundedState(RatStateMachine context, RatStateDictionary ratStateDictionary)
        : base(context, ratStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Grounded, 0, 0);
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        if (Context.IsAggroed)
        {
            _lungeTimer += Time.deltaTime;
            if (_lungeTimer >= Context.LungeTime)
            {
                SwitchState(Dictionary.Lunge());
                _lungeTimer = 0;
            }
        }
        
        if (!Context.IsGrounded) SwitchState(Dictionary.Fall());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
