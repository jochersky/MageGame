using UnityEngine;

[CreateAssetMenu(fileName = "DevourSpellEffectsStrategy", menuName = "Spell Effects Strategies/DevourSpellEffectsStrategy")]
public class DevourSpellEffectsStrategy : PassiveEffectsStrategy
{
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
        passiveSpellAffects.canDevour = true;
    }

    public override void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.canDevour = false;
    }

    public override void CastSpell()
    {
    }

    public override void CastSpell(Transform spawnTransform)
    {
    }
}
