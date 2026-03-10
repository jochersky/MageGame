using UnityEngine;

public class BounceSpell : Spell
{
    [SerializeField] private GameObject projectilePrefab;
    public Transform spawnTransform;
    
    public override void CastSpell()
    {
        if (casting) return;
        Debug.Log("Bounce being casted");
    }
}
