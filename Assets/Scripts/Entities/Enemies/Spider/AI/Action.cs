using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class Action
{
    public string Name { get; }
    // Compared to find the most cost-effective action for the plan
    public float Cost { get; private set; }
    // Decouples actions from being individual classes -> less code to maintain.
    private IActionStrategy actionStrategy;
    public bool Finished => actionStrategy.Finished;
    
    // What the agent needs to be true before starting an action
    public HashSet<Belief> Preconditions { get; } = new();
    // What the agent knows is true after completing an action
    public HashSet<Belief> Effects { get; } = new();

    protected Action(string name)
    {
        Name = name;
    }

    public void Start() => actionStrategy.Start();

    public void Update(float deltaTime)
    {
        if (actionStrategy.CanPerform)
        {
            actionStrategy.Update(deltaTime);
        }
        
        if (!actionStrategy.Finished) return;

        foreach (Belief effect in Effects)
        {
            effect.Evaluate();
        }
    }

    public void Stop() => actionStrategy.Stop();

    public class Builder
    {
        private readonly Action action;

        public Builder(string name)
        {
            action = new Action(name)
            {
                Cost = 1
            };
        }

        public Builder WithCost(float cost)
        {
            action.Cost = cost;
            return this;
        }

        public Builder WithStrategy(IActionStrategy actionStrategy)
        {
            action.actionStrategy = actionStrategy;
            return this;
        }

        public Builder AddPrecondition(Belief precondition)
        {
            action.Preconditions.Add(precondition);
            return this;
        }

        public Builder AddEffect(Belief effect)
        {
            action.Effects.Add(effect);
            return this;
        }

        public Action Build()
        {
            return action;
        }
    }
}
