using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GOAPPlanner
{
    public ActionPlan Plan(GOAPAgent agent, HashSet<Goal> goals, Goal mostRecentGoal = null)
    {
        // Order goals by priority, descending
        // filter goals by which ones have unfinished beliefs,
        // order by priority in descending order (make most recent goal slightly lower in priority)
        List<Goal> orderedGoals = goals
            .Where(g => g.DesiredEffects.Any(b => !b.Evaluate()))
            .OrderByDescending(g=> g == mostRecentGoal ? g.Priority - 0.01 : g.Priority)
            .ToList();
        
        // Try to solve each goal in order
        foreach (Goal goal in orderedGoals)
        {
            Node goalNode = new Node(null, null, goal.DesiredEffects, 0);
            
            // If we can find a path to the goal, return the plan
            if (FindPlan(goalNode, agent.actions))
            {
                // If the goalNode has no leaves and no action to perform try a different goal
                if (goalNode.IsLeafDead) continue;
                
                Stack<Action> actionStack = new Stack<Action>();
                while (goalNode.Leaves.Count > 0)
                {
                    Node cheapestLeaf = goalNode.Leaves.OrderBy(leaf => leaf.Cost).First();
                    goalNode = cheapestLeaf;
                    actionStack.Push(cheapestLeaf.ActionToPerform);
                }

                return new ActionPlan(goal, actionStack, goalNode.Cost);
            }
        }

        return null;
    }
    
    // Search algo to find plan
    // TODO: convert to A*
    private bool FindPlan(Node parent, HashSet<Action> actions)
    {
        var orderedActions = actions.OrderBy(a => a.Cost);

        foreach (Action action in orderedActions)
        {
            var requiredEffects = parent.RequiredEffects;
            
            // Remove any effects that evaluate to true, there is no action to take
            requiredEffects.RemoveWhere(b => b.Evaluate());
            
            // If there are no required effects to fulfill, we have a plan
            if (requiredEffects.Count == 0)
            {
                return true;
            }

            if (action.Effects.Any(requiredEffects.Contains))
            {
                var newRequiredEffects = new HashSet<Belief>(requiredEffects);
                newRequiredEffects.ExceptWith(action.Effects);
                newRequiredEffects.UnionWith(action.Preconditions);

                var newAvailableActions = new HashSet<Action>(actions);
                newAvailableActions.Remove(action);
                
                var newNode = new Node(parent, action, newRequiredEffects, parent.Cost + action.Cost);
                
                // Explore the new node recursively
                if (FindPlan(newNode, newAvailableActions))
                {
                    parent.Leaves.Add(newNode);
                    newRequiredEffects.ExceptWith(newNode.ActionToPerform.Preconditions);
                }
                
                // If all effects at this depth have been satisfied, return true
                if (newRequiredEffects.Count == 0)
                {
                    return true;
                }
            }
        }
        
        return parent.Leaves.Count > 0;
    }

    private Stack<Action> BuildPlan(Node endingAction)
    {
        return new Stack<Action>();
    }
}

public class Node
{
    public Node Parent { get; }
    public Action ActionToPerform { get; }
    public HashSet<Belief> RequiredEffects { get; }
    public List<Node> Leaves { get; }
    public float Cost { get; }

    public bool IsLeafDead => Leaves.Count == 0 && ActionToPerform == null;

    public Node(Node parent, Action actionToPerform, HashSet<Belief> requiredEffects, float cost)
    {
        Parent = parent;
        ActionToPerform = actionToPerform;
        RequiredEffects = new HashSet<Belief>(requiredEffects);
        Leaves = new List<Node>();
        Cost = cost;
    }
}

public class ActionPlan
{
    public Goal Goal { get; }
    public Stack<Action> Actions { get; }
    public float TotalCost { get; set; }

    public ActionPlan(Goal goal, Stack<Action> actions, float totalCost)
    {
        Goal = goal;
        Actions = actions;
        TotalCost = totalCost;
    }
}