using System.Collections;
using UnityEngine;

public class ObscurePresenceSpell : ActiveSpell
{
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(ObscurePresence());
    }

    public override void CastSpell(Vector2 spawnPosition)
    {
        if (casting) return;
        StartCoroutine(ObscurePresence());
    }
    
    private IEnumerator ObscurePresence()
    {
        casting = true;
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
