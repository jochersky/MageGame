using System.Collections;
using UnityEngine;

public class BounceSpell : PassiveSpell
{
    [SerializeField] private int numDoubleJumpsGranted = 1;
    
    // Conditions for casting
    private bool _doubleJumping = false;
    public bool DoubleJumping { get { return _doubleJumping; } set { _doubleJumping = value; CheckCastSpellConditions(); } }
    
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(WindLordsBlessing());
    }

    public override void AddSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.doubleJumps += numDoubleJumpsGranted;
    }

    public override void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.doubleJumps -= numDoubleJumpsGranted;
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
    
    protected override void CheckCastSpellConditions()
    {
        // Passive spell must have all conditions met in order to be cast
        if (!casting && !_doubleJumping) return;
        
        CastSpell();
        
        // Reset casting conditions when spell is cast
        _doubleJumping = false;
    }
    
    private IEnumerator WindLordsBlessing()
    {
        casting = true;
        GameObject inst = Instantiate(projectilePrefab, spawnTransform);
        inst.transform.parent = parentTransform;
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
