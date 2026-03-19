using UnityEngine;

/*
 * Spells which act like passive abilities and most of the time aren't cast directly by the player
 */
public class PassiveSpell : Spell
{
    // Used for casting a PassiveSpell when all conditions are met.
    // Called everytime a condition's value is changed.
    protected virtual void CheckCastSpellConditions() { }
    
    // Add affects to the player's PassiveSpellAffects.
    // Called on equip.
    public virtual void AddSpellAffects(PassiveSpellAffects passiveSpellAffects) { }
    
    // Remove any affects added to the player's PassiveSpellAffects.
    // TODO: Called on unequip
    public virtual void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects) { }
}
