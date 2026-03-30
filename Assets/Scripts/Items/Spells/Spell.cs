using UnityEngine;

public class Spell : Item
{
    [Header("Spell")]
    [SerializeField] protected GameObject projectilePrefab;
    
    public SpellTypes spellType;
    public Transform spawnTransform;
    public Transform parentTransform;
    [SerializeField] protected float spellCooldown;
    protected bool casting = false;
    
    public virtual void CastSpell()
    {
        Debug.Log("Spell being casted");
    }
}
