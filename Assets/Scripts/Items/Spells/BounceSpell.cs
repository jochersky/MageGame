using UnityEngine;

public class BounceSpell : PassiveSpell
{
    public override void CastSpell()
    {
        if (casting) return;
        Debug.Log("Bounce being casted");
    }
}
