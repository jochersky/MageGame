using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellStrategy", menuName = "Spell Strategies/SpellStrategy")]
public class SpellStrategy : ScriptableObject
{
    public float damage;
    public GameObject prefab;
    [Header("Screen Effect Properties")]
    public Color vignetteColor;
    public float vignetteDuration;
    
    private SpellManager _spellManager;

    public virtual void Equip() { }
    public virtual void Equip(PlayerStateMachine playerStateMachine) { }

    public virtual void Equip(SpellManager spellManager, PlayerStateMachine playerStateMachine)
    {
        _spellManager = spellManager;
    }
    
    public virtual void Unequip() { }
    public virtual void CastSpell() { }
    public virtual void Tick(float deltaTime) { }
    public virtual void Cancel() { }

    public virtual GameObject CastSpell(Transform spawnTransform, Vector3 spawnPosition)
    {
        if (vignetteDuration > 0) _spellManager.StartVignetteEffect(vignetteColor, vignetteDuration);
        
        // using spawn transform lets spell be flipped
        GameObject inst = Instantiate(prefab, spawnTransform);
        inst.transform.position = spawnPosition;
        // null so that it won't follow the player's movement 
        inst.transform.parent = null;
        return inst;
    }
}