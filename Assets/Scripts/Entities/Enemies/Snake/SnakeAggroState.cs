using UnityEngine;

public class SnakeAggroState : SnakeBaseState
{
    private float _t;
    private bool _rotated;
    
    public SnakeAggroState(SnakeStateMachine context, SnakeStateDictionary snakeStateDictionary)
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
        
        float progress = (_t * Context.SplineFollowSpeed) % 1;
        
        Context.SnakeGO.transform.position = Context.Spline.EvaluatePosition(progress);

        Vector3 tangent = Context.Spline.EvaluateTangent(progress);
        float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
        Context.SnakeGO.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        _t += Time.fixedDeltaTime;

        Context.Spline.gameObject.transform.position = Vector3.MoveTowards(Context.Spline.gameObject.transform.position, Context.PlayerPosition, Context.SplineFollowSpeed);

        if (progress >= 0.75f && !_rotated)
        {
            Context.Spline.transform.Rotate(0, 0, Random.Range(45, 90));
            _rotated = true;
        }

        if (progress < 0.75f && _rotated) _rotated = false;
        
        if (!Context.PlayerInRange) SwitchState(Dictionary.Idle());
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
