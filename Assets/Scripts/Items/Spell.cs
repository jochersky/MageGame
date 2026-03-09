using UnityEngine;

public class Spell : Item
{
    public SpellTypes spellType;
    [SerializeField] protected float spellCooldown;
    protected bool casting = false;
    
    public virtual void CastSpell()
    {
        Debug.Log("Spell being casted");
    }
}
