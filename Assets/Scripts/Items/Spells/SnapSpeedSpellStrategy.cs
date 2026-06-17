using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SnapSpeedSpellStrategy", menuName = "Spell Strategies/SnapSpeedSpellStrategy")]
public class SnapSpeedSpellStrategy : SpellStrategy
{
    public OperatorTypes operatorType;
    public float buffAmount = 1f;
    public float debuffAmount = 0.25f;
    public float duration = 1f;
    
    private SpellManager _spellManager;
    private PlayerStateMachine _psm;
    private CountdownTimer _debuffTimer;
    
    public override void Equip(SpellManager spellManager, PlayerStateMachine playerStateMachine)
    {
        _spellManager = spellManager;
        _psm = playerStateMachine;
    }

    public override void Tick(float deltaTime)
    {
        if (_debuffTimer is { IsRunning: true }) _debuffTimer.Tick(deltaTime);
    }
    
    public override void CastSpell(Transform spawnTransform, Vector3 spawnPosition)
    {
        // buff
        BasicStatModifier speedBuff = operatorType switch
        {
            OperatorTypes.Add => new BasicStatModifier(StatType.Speed, duration, s => s + buffAmount, name),
            OperatorTypes.Multiply => new BasicStatModifier(StatType.Speed, duration, s => s * buffAmount, name),
            _ => throw new ArgumentOutOfRangeException()
        };
        _psm.Stats.Mediator.AddModifier(speedBuff);
        
        // debuff
        _debuffTimer = new CountdownTimer(duration);
        _debuffTimer.OnTimerStop += ApplySpeedDebuff;
        _debuffTimer.Start();
    }

    private void ApplySpeedDebuff()
    {
        BasicStatModifier speedDebuff = new BasicStatModifier(StatType.Speed, duration, s => s * debuffAmount, "");
        _psm.Stats.Mediator.AddModifier(speedDebuff);
    }
}
