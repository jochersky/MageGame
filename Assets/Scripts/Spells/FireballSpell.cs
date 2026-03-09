using System.Collections;
using UnityEngine;

public class FireballSpell : Spell
{
    [SerializeField] private GameObject projectilePrefab;
    public Transform spawnTransform;
    
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(FuryOfTheDragon());
        Debug.Log("Fireball being casted");
    }
    
    private IEnumerator FuryOfTheDragon()
    {
        casting = true;
        Instantiate(projectilePrefab); 
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
