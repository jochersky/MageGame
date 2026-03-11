using System.Collections;
using UnityEngine;

public class FireballSpell : Spell
{
    [SerializeField] private GameObject projectilePrefab;
    
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(FuryOfTheDragon());
        Debug.Log("Fireball being casted");
    }
    
    private IEnumerator FuryOfTheDragon()
    {
        casting = true;
        GameObject inst = Instantiate(projectilePrefab, spawnTransform);
        inst.transform.parent = parentTransform;
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
