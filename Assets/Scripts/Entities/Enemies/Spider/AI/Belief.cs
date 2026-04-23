using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BeliefFactory
{
    private GOAPAgent agent;
    private readonly Dictionary<string, Belief> beliefs;

    public BeliefFactory(GOAPAgent agent, Dictionary<string, Belief> beliefs)
    {
        this.agent = agent;
        this.beliefs = beliefs;
    }

    public void AddBelief(string key, Func<bool> condition)
    {
        beliefs.Add(key, new Belief.Builder(key)
                            .WithCondition(condition)
                            .Build());
    }
    
    public void AddSensorBelief(string key, Sensor sensor)
    {
        beliefs.Add(key, new Belief.Builder(key)
                            .WithCondition(() => sensor.IsTargetInRangeAndVisible)
                            .WithPosition(() => sensor.TargetPosition())
                            .Build());
    }

    public void AddLocationBelief(string key, float distance, Vector3 position)
    {
        beliefs.Add(key, new Belief.Builder(key)
                            .WithCondition(() => InRangeOf(position, distance))
                            .WithPosition(() => position)
                            .Build());
    }

    bool InRangeOf(Vector2 pos, float range) => Vector2.Distance(agent.transform.position, pos) < range;
}

public class Belief
{
    public string Name { get; }
    
    // Stores the condition we want to check on demand. 
    private Func<bool> condition = () => false;
    private Func<Vector2> seenPosition = () => Vector2.zero;

    public Vector2 Position => seenPosition();
    
    public Belief(string name)
    {
        Name = name;
    }
    
    // Single point of interaction with a belief
    public bool Evaluate() => condition();

    // Used to create different types of beliefs with the same process.
    public class Builder
    {
        private readonly Belief belief;

        public Builder(string name)
        {
            belief = new Belief(name);
        }

        public Builder WithCondition(Func<bool> condition)
        {
            belief.condition = condition;
            return this;
        }

        public Builder WithPosition(Func<Vector2> position)
        {
            belief.seenPosition = position;
            return this;
        }

        public Belief Build()
        {
            return belief;
        }
    }
}
