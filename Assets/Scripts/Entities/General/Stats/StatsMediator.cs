using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsMediator
{
    private readonly LinkedList<StatModifier> modifiers = new();

    // allows passing query initiator details 
    public event EventHandler<Query> Queries;
    public void PerformQuery(object sender, Query query) => Queries?.Invoke(sender, query);

    public void AddModifier(StatModifier modifier)
    {
        // Reset modifiers that are non-stackable for repeat modifiers
        if (!modifier.StackableSource && CheckForDuplicateModifier(modifier)) return;
        
        modifiers.AddLast(modifier);
        Queries += modifier.Handle;

        modifier.OnDispose += _ =>
        {
            modifiers.Remove(modifier);
            Queries -= modifier.Handle;
        };
    }

    public void Update(float deltaTime)
    {
        // update all modifiers with deltatime
        var node = modifiers.First;
        while (node != null)
        {
            var modifier = node.Value;
            modifier.Update(deltaTime);
            node = node.Next;
        }
        
        // Dispose any that are finished, aka Mark and Sweep
        node = modifiers.First;
        while (node != null)
        {
            var nextNode = node.Next;

            if (node.Value.MarkedForRemoval)
            {
                node.Value.Dispose();
            }
            
            node = nextNode;
        }
    }

    private bool CheckForDuplicateModifier(StatModifier newModifier)
    {
        var node = modifiers.First;
        while (node != null)
        {
            StatModifier modifier = node.Value;
            if (modifier.Source == newModifier.Source)
            {
                newModifier.Reset();
                return true;
            }
            node = node.Next;
        }

        return false;
    }
}

public class Query
{
    public readonly StatType StatType;
    public float Value;

    public Query(StatType statType, float value)
    {
        StatType = statType;
        Value = value;
    }
}