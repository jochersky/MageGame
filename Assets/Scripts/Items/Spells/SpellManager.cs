using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private int mana = 100;
    [SerializeField] private Transform spellCastTransform;
    [SerializeField] private Transform spellParentTransform;
    [SerializeField] private PassiveSpellAffects passiveSpellAffects;

    [SerializeField] private bool debug = true;
    [SerializeField] private SpellConfig spell1;
    [SerializeField] private SpellConfig spell2;
    
    private PlayerStateMachine _psm;
    private LayerMask _layerMask;

    public SpellConfig spellConfig1;
    private bool castingSpell1;
    private bool _spell1Part1Casted;

    public SpellConfig spellConfig2;
    private bool castingSpell2;
    private bool _spell2Part1Casted;
    
    public int Mana { get => mana; set => mana = value; }
    
    void Start()
    {
        _psm = GetComponent<PlayerStateMachine>();
        _layerMask = LayerMask.GetMask("Environment");
        
        if (debug) passiveSpellAffects.ClearAffects();
        
        EquipSpell1(spell1);
        EquipSpell2(spell2);
    }

    private void Update()
    {
        if (spellConfig1 && spellConfig1.strategy) spellConfig1.strategy.Tick(Time.deltaTime);
        if (spellConfig2 && spellConfig2.strategy) spellConfig2.strategy.Tick(Time.deltaTime);
    }

    public int AddSpell(SpellConfig spellConfig)
    {
        int spellEquipped = 0;
        
        if (!spellConfig1)
        {
            EquipSpell1(spellConfig);
            spellEquipped = 1;
        }
        else if (!spellConfig2)
        { 
            EquipSpell2(spellConfig);
            spellEquipped = 2;
        }
        
        // PassiveEffectsStrategy effectsStrategy = spellConfig.effectsStrategy;
        // if (effectsStrategy)
        // {
        //     effectsStrategy.AddSpellAffects(passiveSpellAffects);
        //     // effectsStrategy.SubscribeConditions(_psm);
        // }
        
        return spellEquipped;
    }

    public void EquipSpell1(SpellConfig spellConfig)
    {
        if (!spellConfig) return;
        
        UnequipSpell1();
        
        spellConfig1 = spellConfig;
        SpellStrategy strategy = spellConfig1.strategy;
        if (strategy)
        {
            spellConfig1.strategy.Equip();
            spellConfig1.strategy.Equip(_psm);
            spellConfig1.strategy.Equip(this, _psm);
        }
        
        PassiveEffectsStrategy effectsStrategy = spellConfig1.effectsStrategy;
        if (effectsStrategy)
        {
            effectsStrategy.AddSpellAffects(passiveSpellAffects);
            effectsStrategy.SubscribeConditions(_psm);
        }
    }

    public void EquipSpell2(SpellConfig spellConfig)
    {
        if (!spellConfig) return;
        
        UnequipSpell2();
        
        spellConfig2 = spellConfig;
        SpellStrategy strategy = spellConfig2.strategy;
        if (strategy)
        {
            spellConfig2.strategy.Equip();
            spellConfig2.strategy.Equip(_psm);
            spellConfig2.strategy.Equip(this, _psm);
        }
        
        PassiveEffectsStrategy effectsStrategy = spellConfig2.effectsStrategy;
        if (effectsStrategy)
        {
            effectsStrategy.AddSpellAffects(passiveSpellAffects);
            effectsStrategy.SubscribeConditions(_psm);
        }
    }

    public void UnequipSpell1()
    {
        if (!spellConfig1) return;
        
        if (spellConfig1.strategy) spellConfig1.strategy.Unequip();
        if (spellConfig1.effectsStrategy)
        {
            spellConfig1.effectsStrategy.RemoveSpellAffects(passiveSpellAffects);
            spellConfig1.effectsStrategy.UnsubscribeConditions(_psm);
        }

        spellConfig1 = null;
    }
    
    public void UnequipSpell2()
    {
        if (!spellConfig2) return;
        
        if (spellConfig2.strategy) spellConfig2.strategy.Unequip();
        if (spellConfig2.effectsStrategy)
        {
            spellConfig2.effectsStrategy.RemoveSpellAffects(passiveSpellAffects);
            spellConfig2.effectsStrategy.UnsubscribeConditions(_psm);
        }
        
        spellConfig2 = null;
    }

    public void OnSpell1Pressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || _psm.IsDead || castingSpell1 || !spellConfig1 || !spellConfig1.strategy) return;
        
        if (spellConfig1.manaCost > mana) return;

        // two-part-cast spells should only use one cast worth of mana
        if (spellConfig1.twoPartCast)
        {
            if (!_spell1Part1Casted)
            {
                _spell1Part1Casted = true;
                mana -= spellConfig1.manaCost;
            }
            else
            {
                _spell1Part1Casted = false;
            }
        }
        else
        {
            mana -= spellConfig1.manaCost;
        }
        
        // Player will get hit by their own spell if they cast it towards a wall
        Vector2 dir = spellCastTransform.position - transform.position;
        Debug.DrawRay(transform.position, dir, Color.red, 5);
        if (spellConfig1.changePositionOnObstruction && Physics2D.Raycast(transform.position, dir, dir.magnitude, _layerMask)) 
            spellConfig1.strategy.CastSpell(spellCastTransform, _psm.gameObject.transform.position);
        else 
            spellConfig1.strategy.CastSpell(spellCastTransform, spellCastTransform.position);
        
        StartCoroutine(WaitBeforeCastingSpell1Again());
    }
    
    public void OnSpell2Pressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || _psm.IsDead || castingSpell2 || !spellConfig2 || !spellConfig2.strategy) return;
        
        if (spellConfig2.manaCost > mana) return;
        
        // two-part-cast spells should only use one cast worth of mana
        if (spellConfig2.twoPartCast)
        {
            if (!_spell2Part1Casted)
            {
                _spell2Part1Casted = true;
                mana -= spellConfig2.manaCost;
            }
            else
            {
                _spell2Part1Casted = false;
            }
        }
        else
        {
            mana -= spellConfig2.manaCost;
        }
        
        // Player will get hit by their own spell if they cast it towards a wall
        Vector2 dir = spellCastTransform.position - transform.position;
        if (spellConfig2.changePositionOnObstruction && Physics2D.Raycast(transform.position, dir, dir.magnitude, _layerMask)) 
            spellConfig2.strategy.CastSpell(spellCastTransform, _psm.gameObject.transform.position);
        else 
            spellConfig2.strategy.CastSpell(spellCastTransform, spellCastTransform.position);
        
        StartCoroutine(WaitBeforeCastingSpell2Again());
    }
    
    private IEnumerator WaitBeforeCastingSpell1Again()
    {
        castingSpell1 = true;
        yield return new WaitForSeconds(spellConfig1.cooldown);
        castingSpell1 = false;
    }
    
    private IEnumerator WaitBeforeCastingSpell2Again()
    {
        castingSpell2 = true;
        yield return new WaitForSeconds(spellConfig2.cooldown);
        castingSpell2 = false;
    }
    
    public IEnumerator CancelSpellAfterDuration(float duration, SpellStrategy spellStrategy)
    {
        yield return new WaitForSeconds(duration);
        spellStrategy.Cancel();
    }
}
