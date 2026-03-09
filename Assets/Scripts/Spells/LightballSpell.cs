using System.Collections;
using UnityEngine;

public class LightballSpell : Spell
{
    [SerializeField] private GameObject projectilePrefab;
    public Transform spawnTransform;
    
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(GiftOfLight());
        Debug.Log("Light being casted");
    }
    
    private IEnumerator GiftOfLight()
    {
        Instantiate(projectilePrefab);
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
