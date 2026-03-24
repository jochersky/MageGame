using System.Collections;
using UnityEngine;

public class FireballSpell : ActiveSpell
{
    private Vector2 _spawnPosition;
    
    public override void CastSpell()
    {
        if (casting) return;
        StartCoroutine(FuryOfTheDragon());
    }

    public override void CastSpell(Vector2 spawnPosition)
    {
        if (casting) return;
        _spawnPosition = spawnPosition;
        StartCoroutine(FuryOfTheDragon());
    }
    
    private IEnumerator FuryOfTheDragon()
    {
        casting = true;
        GameObject inst = Instantiate(projectilePrefab, spawnTransform);
        if (_spawnPosition != Vector2.zero)
        {
            inst.transform.position = _spawnPosition;
            _spawnPosition = Vector2.zero;
        }
        inst.transform.parent = parentTransform;
        yield return new WaitForSeconds(spellCooldown);
        casting = false;
    }
}
