using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FeatheryDiveSpellStrategy", menuName = "Spell Strategies/FeatheryDiveSpellStrategy")]
public class FeatheryDiveSpellStrategy : SpellStrategy
{
    public OperatorTypes operatorType;
    public float gravityFactor = 1f;
    public float duration = 1f;
    
    private PlayerStateMachine _psm;
    
    public override void Equip(PlayerStateMachine playerStateMachine)
    {
        _psm = playerStateMachine;
    }
    
    public override void CastSpell(Transform spawnTransform, Vector3 spawnPosition)
    {
        BasicStatModifier gravityMod = operatorType switch
        {
            OperatorTypes.Add => new BasicStatModifier(StatType.GravityFactor, duration, s => s + gravityFactor, name),
            OperatorTypes.Multiply => new BasicStatModifier(StatType.GravityFactor, duration, s => s * gravityFactor, name),
            _ => throw new ArgumentOutOfRangeException()
        };
        _psm.Stats.Mediator.AddModifier(gravityMod);
    }
}
