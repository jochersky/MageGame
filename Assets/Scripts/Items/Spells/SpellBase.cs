using UnityEngine;

public interface ISpell
{
    void Initialize(SpellConfig spellConfig);
    void Equip();
    void Unequip();
    void Cast(Transform target);
}

public class SpellBase : MonoBehaviour, ISpell
{
    protected SpellConfig config;
    
    protected bool casting = false;
    
    public virtual void Initialize(SpellConfig spellConfig)
    {
        config = spellConfig;
    }

    public virtual void Equip()
    {
        Debug.Log($"{config.itemName} equipped.");
    }

    public virtual void Unequip()
    {
        Debug.Log($"{config.itemName} equipped.");
    }

    public virtual void Cast(Transform target)
    {
        Debug.Log($"{config.itemName} used.");
    }
}
