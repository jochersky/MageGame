using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GOAPAgent : MonoBehaviour
{
    [Header("Sensors")] 
    [SerializeField] private Sensor chaseSensor;
    [SerializeField] private Sensor attackSensor;
    
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
    
    void Awake()
    {
        SetupBeliefs();
        SetupActions();
        SetupGoals();

        goalPlanner = new GOAPPlanner();
    }

    private void SetupBeliefs()
    {
        beliefs = new Dictionary<string, Belief>();
        BeliefFactory factory = new BeliefFactory(this, beliefs);
        
        factory.AddBelief("Nothing", () => false);
        
        factory.AddBelief("AgentIdle", () => !navMeshAgent.hasPath);
        factory.AddBelief("AgentMoving", () => navMeshAgent.hasPath);
        factory.AddBelief("AttackingEnemy", () => false); // agent can always be attacking an enemy

        factory.AddSensorBelief("EnemyInChaseRange", chaseSensor);
        factory.AddSensorBelief("EnemyInAttackRange", attackSensor);
    }

    private void SetupActions()
    {
        actions = new HashSet<Action>();

        actions.Add(new Action.Builder("Idle")
            .WithStrategy(new IdleActionStrategy(animationManager, 5))
            .AddEffect(beliefs["Nothing"])
            .Build());

        actions.Add(new Action.Builder("ChaseEnemy")
            .WithStrategy(new MoveActionStrategy(animationManager, navMeshAgent, () => beliefs["EnemyInChaseRange"].Position))
            .AddPrecondition(beliefs["EnemyInChaseRange"])
            .AddEffect(beliefs["EnemyInAttackRange"])
            .Build());

        actions.Add(new Action.Builder("AttackEnemy")
            .WithStrategy(new AttackActionStrategy(animationManager))
            .AddPrecondition(beliefs["EnemyInAttackRange"])
            .AddEffect(beliefs["AttackingEnemy"])
            .Build());
    }

    private void SetupGoals()
    {
        goals = new HashSet<Goal>();
        
        goals.Add(new Goal.Builder("WaitForMotivation")
            .WithPriority(1)
            .WithDesiredEffect(beliefs["Nothing"])
            .Build());
        
        goals.Add(new Goal.Builder("KillEnemy")
            .WithPriority(10)
            .WithDesiredEffect(beliefs["AttackingEnemy"])
            .Build());
    }

    void Update()
    {
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

            if (currentAction.Finished)
            {
                currentAction.Stop();
                currentAction = null;

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
