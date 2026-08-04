using System;
using UnityEngine;

public class ManaRestorative : Pickup
{
    [SerializeField] int manaRestored;
    public override void PickUpEffect()
    {
        GameManager.Instance.SpellManager.UpdateMana(manaRestored);
    }
}
