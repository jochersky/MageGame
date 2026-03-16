using UnityEngine;

/*
 * Spells which act like passive abilities and most of the time aren't cast directly by the player
 */
public class PassiveSpell : Spell
{
    [Header("Passive Spell")] 
    public SpellTypes[] spells;
}
