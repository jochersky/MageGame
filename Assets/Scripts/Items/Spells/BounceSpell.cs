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
        Debug.Log("Bounce being casted");
    }

    public override void AddSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.doubleJumps += numDoubleJumpsGranted;
    }

    public override void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects)
    {
        passiveSpellAffects.doubleJumps -= numDoubleJumpsGranted;
    }

    protected override void CheckCastSpellConditions()
    {
        // Passive spell must have all conditions met in order to be cast
        if (!_doubleJumping) return;
        
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
