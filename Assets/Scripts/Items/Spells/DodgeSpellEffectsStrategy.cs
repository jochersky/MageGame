using UnityEngine;

[CreateAssetMenu(fileName = "DodgeSpellEffectsStrategy", menuName = "Spell Effects Strategies/DodgeSpellEffectsStrategy")]
public class DodgeSpellEffectsStrategy : PassiveEffectsStrategy
{
    public int numDodgesGranted = 1;
    
    protected override void CheckCastSpellConditions()
    {
    }

    public override void SubscribeConditions(PlayerStateMachine psm)
    {
    }

    public override void UnsubscribeConditions(PlayerStateMachine psm)
    {
    }

    public override void AddSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.dodges += numDodgesGranted;
    }

    public override void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.dodges -= numDodgesGranted;
    }

    public override void CastSpell()
    {
    }

    public override void CastSpell(Transform spawnTransform)
    {
    }
}
