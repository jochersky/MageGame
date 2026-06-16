using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class GOAPAction
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

    protected GOAPAction(string name)
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
        private readonly GOAPAction _goapAction;

        public Builder(string name)
        {
            _goapAction = new GOAPAction(name)
            {
                Cost = 1
            };
        }

        public Builder WithCost(float cost)
        {
            _goapAction.Cost = cost;
            return this;
        }

        public Builder WithStrategy(IActionStrategy actionStrategy)
        {
            _goapAction.actionStrategy = actionStrategy;
            return this;
        }

        public Builder AddPrecondition(Belief precondition)
        {
            _goapAction.Preconditions.Add(precondition);
            return this;
        }

        public Builder AddEffect(Belief effect)
        {
            _goapAction.Effects.Add(effect);
            return this;
        }

        public GOAPAction Build()
        {
            return _goapAction;
        }
    }
}
