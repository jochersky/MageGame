using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class StatusEffectManager : MonoBehaviour
{
    private Health _health;
    private StatsMediator _statsMediator;
    
    public void Initialize(StatsMediator statsMediator)
    {
        _statsMediator = statsMediator;
        
        _health = GetComponent<Health>();
        _health.OnDamageTaken += properties => ApplyStatusEffect(properties.effect);
    }

    private async void ApplyStatusEffect(StatusEffect statusEffect)
    {
        BasicStatModifier mod = statusEffect.operatorType switch
        {
            OperatorTypes.Add => new BasicStatModifier(statusEffect.statType, statusEffect.duration, s => s + statusEffect.amount, name),
            OperatorTypes.Multiply => new BasicStatModifier(statusEffect.statType, statusEffect.duration, s => s * statusEffect.amount, name),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (statusEffect.delay == 0) _statsMediator.AddModifier(mod);
        else
        {
            await Awaitable.WaitForSecondsAsync(statusEffect.delay);
            _statsMediator.AddModifier(mod);
        }
    }
}
