using UnityEngine;

[CreateAssetMenu(fileName = "BounceSpellEffectsStrategy", menuName = "Spell Effects Strategies/BounceSpellEffectsStrategy")]
public class BounceSpellEffectsStrategy : PassiveEffectsStrategy
{
    public int numDoubleJumpsGranted = 1;
    
    // Conditions for casting
    private bool _doubleJumping = false;
    public bool DoubleJumping { get { return _doubleJumping; } set { _doubleJumping = value; CheckCastSpellConditions(); } }

    protected override void CheckCastSpellConditions()
    {
        // Passive spell must have all conditions met in order to be cast
        if (!_doubleJumping) return;
        
        CastSpell();
        
        // Reset casting conditions when spell is cast
        _doubleJumping = false;
    }

    public override void SubscribeConditions(PlayerStateMachine psm)
    {
        psm.OnDoubleJumpComplete += UpdateDoubleJump;
    }

    public override void UnsubscribeConditions(PlayerStateMachine psm)
    {
        psm.OnDoubleJumpComplete -= UpdateDoubleJump;
    }

    private void UpdateDoubleJump()
    {
        DoubleJumping = true;
    }
    
    public override void AddSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.doubleJumps += numDoubleJumpsGranted;
    }

    public override void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.doubleJumps -= numDoubleJumpsGranted;
    }

    public override void CastSpell() { }

    public override void CastSpell(Transform spawnTransform)
    {
        GameObject inst = Instantiate(prefab, spawnTransform);
    }
}