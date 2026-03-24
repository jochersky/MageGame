using UnityEngine;

/*
 * Spells which are directly cast by the player using LMB or RMB (or whatever they're remapped as)
 */
public class ActiveSpell : Spell
{
    public bool changePositionOnObstruction;
    
    public virtual void CastSpell(Vector2 spawnPosition)
    {
        Debug.Log("Spell being casted using unique spawn transform");
    }
}
