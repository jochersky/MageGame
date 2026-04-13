using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class GOAPAgent : MonoBehaviour
{
    [Header("Sensors")] 
    [SerializeField] private Sensor chaseSensor;
    [SerializeField] private Sensor attackSensor;

    [Header("Stats")] 
    [SerializeField] private Health health;
    [SerializeField] private Hitbox hitbox;
    
    private GOAPPlanner planner;
    private NavMeshAgent navMeshAgent;
    private AnimationManager animationManager;

    private Goal lastGoal;
    public Goal currentGoal;
    public ActionPlan actionPlan;
    public Action currentAction;
    
    public Dictionary<string, Belief> beliefs;
    public HashSet<Action> actions;
    public HashSet<Goal> goals;

    private GOAPPlanner goalPlanner;

    // normal
    private int _killEnemyInitialPriority = 3;
    private int _stayAwayFromDangerInitialPriority = 1;
    // low health
    private int _killEnemyPriorityWithLowHealth = 1;
    private int _stayAwayFromDangerPriorityWithLowHealth = 2;
    
    private bool _dead = false;
    private float _previousHealth;
    private Vector3 _origin;
    private GameObject _currentTarget;
    private Health _currentTargetHealth;
    
    
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        
        animationManager = GetComponent<AnimationManager>();

        goalPlanner = new GOAPPlanner();
    }

    private void Start()
    {
        _origin = transform.position;
        _previousHealth = health.CurrentHealth;
        
        chaseSensor.OnTargetChanged += HandleTargetChanged;
        health.OnDeath += () => _dead = true;
        health.OnDeath += () => hitbox.Disable();
        health.OnHealthChanged += HandleHealthChanged;
            
        SetupBeliefs();
        SetupActions();
        SetupGoals();
    }

    private void SetupBeliefs()
    {
        beliefs = new Dictionary<string, Belief>();
        BeliefFactory factory = new BeliefFactory(this, beliefs);
        
        factory.AddBelief("Nothing", () => false);
        
        factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
        factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);
        factory.AddBelief("AttackingEnemy", () => false); // agent can always be attacking an enemy
        factory.AddBelief("AvoidingEnemy", () => false); // agent can always be avoiding
        factory.AddBelief("LowHealth", () => health.CurrentHealth <= 2);
        factory.AddBelief("GainedHealth", () => false); // agent can always be gaining health
        factory.AddBelief("TargetDead", () => _currentTargetHealth && _currentTargetHealth.CurrentHealth <= 0);
        
        factory.AddLocationBelief("AgentAtOrigin", 0.1f, _origin);
        
        factory.AddSensorBelief("EnemyInChaseRange", chaseSensor);
        factory.AddSensorBelief("EnemyInAttackRange", attackSensor);
    }

    private void SetupActions()
    {
        actions = new HashSet<Action>();

        actions.Add(new Action.Builder("Idle")
            .WithStrategy(new IdleActionStrategy(animationManager, 1))
            .AddEffect(beliefs["Nothing"])
            .Build());

        actions.Add(new Action.Builder("ReturnToOrigin")
            .WithStrategy(new MoveActionStrategy(animationManager, navMeshAgent, () => _origin))
            .AddEffect(beliefs["AgentAtOrigin"])
            .AddEffect(beliefs["AgentMoving"])
            .Build());

        actions.Add(new Action.Builder("RunFromEnemy")
            .WithStrategy(new MoveActionStrategy(animationManager, navMeshAgent, () => _origin))
            .AddPrecondition(beliefs["LowHealth"])
            .AddEffect(beliefs["AvoidingEnemy"])
            .AddEffect(beliefs["AgentMoving"])
            .Build());

        actions.Add(new Action.Builder("ChaseEnemy")
            .WithStrategy(new MoveActionStrategy(animationManager, navMeshAgent, () => beliefs["EnemyInChaseRange"].Position))
            .AddPrecondition(beliefs["EnemyInChaseRange"])
            .AddEffect(beliefs["EnemyInAttackRange"])
            .AddEffect(beliefs["AgentMoving"])
            .Build());

        actions.Add(new Action.Builder("AttackEnemy")
            .WithStrategy(new AttackActionStrategy(animationManager))
            .AddPrecondition(beliefs["EnemyInAttackRange"])
            .AddEffect(beliefs["TargetDead"])
            .Build());

        // actions.Add(new Action.Builder("Consume")
        //     .AddPrecondition(beliefs["EnemyInAttackRange"])
        //     .AddPrecondition(beliefs["DeadEnemyPositionKnown"])
        //     .AddEffect(beliefs["GainedHealth"])
        //     .Build());
    }

    private void SetupGoals()
    {
        goals = new HashSet<Goal>();
        
        goals.Add(new Goal.Builder("WaitForMotivation")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Nothing"])
            .WithDesiredEffect(beliefs["AgentAtOrigin"])
            .Build());
        
        goals.Add(new Goal.Builder("KillEnemy")
            .WithPriority(_killEnemyInitialPriority)
            .WithDesiredEffect(beliefs["TargetDead"])
            .Build());

        goals.Add(new Goal.Builder("StayAwayFromDanger")
            .WithPriority(_stayAwayFromDangerInitialPriority)
            .WithDesiredEffect(beliefs["AvoidingEnemy"])
            .Build());

        // goals.Add(new Goal.Builder("GainHealth")
        //     .WithPriority(1)
        //     .WithDesiredEffect(beliefs["GainedHealth"])
        //     .Build());
    }

    private void RecomputeGoal()
    {
        currentAction = null;
        currentGoal = null;
    }

    private void HandleTargetChanged(GameObject newTarget)
    {
        if (newTarget)
        {
            Health h = newTarget.GetComponentInChildren<Health>();
            _currentTargetHealth = h;
        }
        else
        {
            _currentTargetHealth = null;
        }
        _currentTarget = newTarget;
        
        RecomputeGoal();
    }

    private void HandleHealthChanged(int newHealth)
    {
        if (_previousHealth > 2 && newHealth <= 2)
        {
            foreach (Goal goal in goals)
            {
                if (goal.Name == "KillEnemy")
                    goal.UpdatePriority(_killEnemyPriorityWithLowHealth);
                else if (goal.Name == "StayAwayFromDanger")
                    goal.UpdatePriority(_stayAwayFromDangerPriorityWithLowHealth);
            }
        }
        else if (_previousHealth <= 2 && newHealth > 2)
        {
            foreach (Goal goal in goals)
            {
                if (goal.Name == "KillEnemy")
                    goal.UpdatePriority(_killEnemyInitialPriority);
                else if (goal.Name == "StayAwayFromDanger")
                    goal.UpdatePriority(_stayAwayFromDangerInitialPriority);
            }
        }
        RecomputeGoal();
    }

    void Update()
    {
        if (_dead) return;
        
        // Update the plan and current action if there is one
        if (currentAction == null)
        {
            CalculatePlan();

            if (actionPlan != null && actionPlan.Actions.Count > 0)
            {
                navMeshAgent.ResetPath();

                currentGoal = actionPlan.Goal;
                currentAction = actionPlan.Actions.Pop();
                // Verify all precondition effects are true
                if (currentAction.Preconditions.All(b => b.Evaluate()))
                {
                    currentAction.Start();
                }
                else
                {
                    currentAction = null;
                    currentGoal = null;
                }
            }
        }
        
        // If we have a current action, execute it
        if (actionPlan != null && currentAction != null)
        {
            currentAction.Update(Time.deltaTime);
            
            // TODO: update look rotation of agent
            if (navMeshAgent.velocity.magnitude >= 0)
            {
                Vector3 moveDirection = new Vector3(navMeshAgent.velocity.x, navMeshAgent.velocity.y, 0f);
                if (moveDirection != Vector3.zero)
                {
                    float angle = Mathf.Atan2(moveDirection.x, moveDirection.y) * Mathf.Rad2Deg;

                    Quaternion lookRotation = Quaternion.AngleAxis(angle, Vector3.back);
                    // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
                    transform.rotation = lookRotation;
                }
            }

            if (currentAction.Finished)
            {
                currentAction.Stop();
                currentAction = null;

                // TODO: create function that runs through and evaluates all beliefs to activate other behavior
                if (beliefs["TargetDead"].Evaluate())
                {
                    chaseSensor.RemoveCurrentTarget();
                }

                if (actionPlan.Actions.Count == 0)
                {
                    lastGoal = currentGoal;
                    currentGoal = null;
                }
            }
        }
    }

    private void CalculatePlan()
    {
        float priorityLevel = currentGoal?.Priority ?? 0;

        HashSet<Goal> goalsToCheck = goals;
        
        // If we have a current goal, we only want to check goals with higher priority
        if (currentGoal != null)
        {
            goalsToCheck = new HashSet<Goal>(goals.Where(g => g.Priority > priorityLevel));
        }
        
        ActionPlan potentialPlan = goalPlanner.Plan(this, goalsToCheck, lastGoal);
        if (potentialPlan != null)
        {
            actionPlan = potentialPlan;
        }
    }
}
