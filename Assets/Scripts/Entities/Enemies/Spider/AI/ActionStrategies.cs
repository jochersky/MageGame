using System;
using UnityEngine;
using UnityEngine.AI;

public interface IActionStrategy
{
    bool CanPerform { get; }
    bool Finished { get; }
    
    void Start() { }
    void Update(float deltaTime) { }
    void Stop() { }
}

public class IdleActionStrategy : IActionStrategy
{
    private AnimationManager animationManager;
    
    private float timer = 1f;
    private float elapsedTime = 0f;
    
    public bool CanPerform => true;
    public bool Finished { get; private set; }

    public IdleActionStrategy(AnimationManager animationManager, float timer)
    {
        this.animationManager = animationManager;
        this.timer = timer;
    }
    
    public void Start()
    {
        animationManager.Idle();
        elapsedTime = 0f;
    } 

    public void Update(float deltaTime)
    {
        while (elapsedTime < timer)
        {
            elapsedTime += deltaTime;
            if (elapsedTime >= timer) Finished = true;
        }
    }
}

public class MoveActionStrategy : IActionStrategy
{
    private AnimationManager animationManager;
    private NavMeshAgent agent;
    private Func<Vector2> destination;

    public bool CanPerform => !Finished;
    // TODO: update to have stopping distance be a range that can be set in a SO
    public bool Finished => agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending;

    public MoveActionStrategy(AnimationManager animationManager, NavMeshAgent agent, Func<Vector2> destination)
    {
        this.animationManager = animationManager;
        this.agent = agent;
        this.destination = destination;
    }

    public void Start()
    {
        animationManager.Move();
        agent.SetDestination(destination());
    }

    public void Update(float deltaTime) { }
    public void Stop() => agent.ResetPath();
}

public class AttackActionStrategy : IActionStrategy
{
    private AnimationManager animationManager;
    
    private float timer = 1f;
    private float elapsedTime = 0f;
    
    public bool CanPerform { get; }
    public bool Finished { get; private set; }

    public AttackActionStrategy(AnimationManager animationManager)
    {
        this.animationManager = animationManager;
        // this.timer = animationManager.GetAnimationDuration(animationManager.AttackHash);
        this.timer = 0.15f;
    }

    public void Start()
    {
        animationManager.Attack();
        elapsedTime = 0f;
    } 

    public void Update(float deltaTime)
    {
        while (elapsedTime < timer)
        {
            elapsedTime += deltaTime;
            if (elapsedTime >= timer) Finished = true;
        }
    }
}
