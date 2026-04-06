using System.Collections.Generic;
using UnityEngine;

public class Goal
{
    public string Name { get; }
    public int Priority { get; private set; }
    public HashSet<Belief> DesiredEffects { get; } = new();

    Goal(string name)
    {
        Name = name;
    }

    public class Builder
    {
        private readonly Goal goal;

        public Builder(string name)
        {
            goal = new Goal(name);
        }

        public Builder WithPriority(int priority)
        {
            goal.Priority = priority;
            return this;
        }

        public Builder WithDesiredEffect(Belief belief)
        {
            goal.DesiredEffects.Add(belief);
            return this;
        }

        public Goal Build()
        {
            return goal;
        }
    }
}
