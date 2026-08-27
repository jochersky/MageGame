using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    public PlayerClimbState(PlayerStateMachine context, PlayerStateDictionary playerStateDictionary)
        : base(context, playerStateDictionary)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Context.Animator.CrossFade(Context.Climbing, 0, 0);
        
        Context.IsClimbingRope = false;
        Context.Rigidbody.gravityScale = 0;
        Context.LinearVelocityY = 0;
        Context.HorizontalMovement = 0;
        // Set so that player can jump when climbing.
        Context.WasClimbing = true;

        Context.ClimbDir = Context.MoveDirection;

        Context.StartClimbDelay();
    }

    public override void UpdateState()
    {
        if (Context.IsDead) SwitchState(Dictionary.Dead());
        
        Vector2 pos = Context.transform.position;
        if ((pos - Context.ClimbPosition).magnitude >= 0.1f)
            Context.transform.position = Vector3.MoveTowards(pos, Context.ClimbPosition, Context.ClimbSnapSpeed);
        
        if (Context.NewJumpPress) SwitchState(Dictionary.Jump());
        else if (Context.VerticalDirection.y < 0)
        {
            // Context.WasClimbing = false;
            SwitchState(Dictionary.Fall());
        }
    }

    public override void ExitState()
    {
        Context.CheckForFlipTransform();
    }

    public override void InitializeSubState()
    {
    }

    private void SnapToClimbPosition()
    {
    }
    
    public override string ToString()
    {
        return "PlayerClimbState";
    }
}
