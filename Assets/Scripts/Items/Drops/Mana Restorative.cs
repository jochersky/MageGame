using System;
using UnityEngine;

public class ManaRestorative : Pickup
{
    [SerializeField] int manaRestored;
    public override void PickUpEffect()
    {
        FindAnyObjectByType<SpellManager>().UpdateMana(manaRestored);
    }
}
