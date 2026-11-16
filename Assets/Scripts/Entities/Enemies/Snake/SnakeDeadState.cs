using UnityEngine;

public class SnakeDeadState : SnakeBaseState
{
    private float _t;
    
    public SnakeDeadState(SnakeStateMachine currentContext, SnakeStateDictionary ratStateDictionary) 
        : base(currentContext, ratStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Chain.ResetProgress();
    }

    public override void UpdateState()
    {
        Vector3 fallDirection = Vector3.down * 0.005f;
        Context.Chain.Head.transform.position += fallDirection + Random.insideUnitSphere * 0.01f;
        Context.Chain.UpdateChainFollowingHead(fallDirection, 1f);
        
        _t += Time.deltaTime;
        if (_t >= Context.FallDuration)
            Context.Destroy();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
    }
}
