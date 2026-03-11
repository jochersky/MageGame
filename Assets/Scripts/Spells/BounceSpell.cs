using UnityEngine;

public class BounceSpell : Spell
{
    [SerializeField] private GameObject projectilePrefab;
    
    public override void CastSpell()
    {
        if (casting) return;
        Debug.Log("Bounce being casted");
    }
}
