using System;
using UnityEngine;

public class ManaRestorative : Pickup
{
    [SerializeField] int manaRestored;
    public override void PickUpEffect()
    {
        SpellManager player = FindFirstObjectByType<SpellManager>();
        player.AddMana(manaRestored);
    }
}
