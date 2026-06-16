using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SnapSpeedSpellStrategy", menuName = "Spell Strategies/SnapSpeedSpellStrategy")]
public class SnapSpeedSpellStrategy : SpellStrategy
{
    public OperatorTypes operatorType;
    public float amount = 1f;
    public float duration = 1f;
    
    private SpellManager _spellManager;
    private PlayerStateMachine _psm;
    
    private BasicStatModifier _modifier;

    public override void Equip(SpellManager spellManager, PlayerStateMachine playerStateMachine)
    {
        _spellManager = spellManager;
        _psm = playerStateMachine;
    }

    public override void CastSpell(Transform spawnTransform, Vector3 spawnPosition)
    {
        _modifier = operatorType switch
        {
            OperatorTypes.Add => new BasicStatModifier(StatType.Speed, duration, s => s + amount, name),
            OperatorTypes.Multiply => new BasicStatModifier(StatType.Speed, duration, s => s * amount, name),
            _ => throw new ArgumentOutOfRangeException()
        };
        _psm.Stats.Mediator.AddModifier(_modifier);
    }
}
