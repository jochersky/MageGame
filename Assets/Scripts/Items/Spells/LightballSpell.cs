using System.Collections;
using UnityEngine;

public class LightballSpell : ActiveSpell
{
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(GiftOfLight());
        Debug.Log("Light being casted");
    }
    
    private IEnumerator GiftOfLight()
    {
        casting = true;
        GameObject inst = Instantiate(projectilePrefab, spawnTransform);
        inst.transform.parent = parentTransform;
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
