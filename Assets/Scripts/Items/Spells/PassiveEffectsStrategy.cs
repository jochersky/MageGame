using UnityEngine;

public abstract class PassiveEffectsStrategy : ScriptableObject
{
    public GameObject prefab;
    
    // Used for casting a PassiveSpell when all conditions are met.
    // Called everytime a condition's value is changed.
    protected abstract void CheckCastSpellConditions();
    
    // Used to subscribe methods to update conditions based on player's actions on equip.
    public abstract void SubscribeConditions(PlayerStateMachine psm);
    
    // Used to unsubscribe methods from player events on unequip.
    public abstract void UnsubscribeConditions(PlayerStateMachine psm);
    
    // Add affects to the player's PassiveSpellAffects.
    // Called on equip.
    public abstract void AddSpellAffects(PassiveSpellAffects passiveSpellAffects);
    
    // Remove any affects added to the player's PassiveSpellAffects.
    public abstract void RemoveSpellAffects(PassiveSpellAffects passiveSpellAffects);
    
    public abstract void CastSpell();
    public abstract void CastSpell(Transform spawnTransform);
}