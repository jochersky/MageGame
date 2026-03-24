using UnityEngine;
using UnityEngine.InputSystem;

public enum SpellTypes
{
    GiftOfLight,
    WindLordsBlessing,
    FuryOfTheDragon
}

[RequireComponent(typeof(PlayerStateMachine))]
public class SpellManager : MonoBehaviour
{
    [SerializeField] private bool debug = true;
    [SerializeField] private PassiveSpellAffects passiveSpellAffects;
    [SerializeField] private Transform spellCastTransform;
    [SerializeField] private Transform spellParentTransform;
    private PlayerStateMachine _psm;
    public ActiveSpell equippedSpell1;
    public ActiveSpell equippedSpell2;
    public PassiveSpell equippedPassiveSpell1;
    public PassiveSpell equippedPassiveSpell2;

    public static SpellManager Instance { get; private set; }
    
    private LayerMask _layerMask;
    
    private void Awake()
    {
        // Ensure only one instance of the inventory exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _psm = GetComponent<PlayerStateMachine>();
        _layerMask = LayerMask.GetMask("Environment");
        
        if (debug) passiveSpellAffects.ClearAffects();
    }
    
    public void OnSpell1Pressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || _psm.IsDead) return;
        if (!equippedSpell1) return;
        
        // Player will get hit by their own spell if they cast it towards a wall
        Vector2 dir = spellCastTransform.position - transform.position;
        if (equippedSpell1.changePositionOnObstruction && Physics2D.Raycast(transform.position, dir, dir.magnitude, _layerMask)) 
            equippedSpell1.CastSpell(_psm.gameObject.transform.position);
        else 
            equippedSpell1.CastSpell();
    }
    
    public void OnSpell2Pressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || _psm.IsDead) return;
        if (!equippedSpell2) return;
        
        // Player will get hit by their own spell if they cast it towards a wall
        Vector2 dir = spellCastTransform.position - transform.position;
        if (equippedSpell1.changePositionOnObstruction && Physics2D.Raycast(transform.position, dir, dir.magnitude, _layerMask)) 
            equippedSpell2.CastSpell(_psm.gameObject.transform.position);
        else 
            equippedSpell2.CastSpell();
    }

    public void EquipSpell1(ActiveSpell spell)
    {
        equippedSpell1 = spell;
        spell.spawnTransform = spellCastTransform;
        spell.parentTransform = spellParentTransform;
    }
    
    public void EquipSpell2(ActiveSpell spell)
    {
        equippedSpell2 = spell;
        spell.spawnTransform = spellCastTransform;
        spell.parentTransform = spellParentTransform;
    }
    
    // TODO
    public void UnequipSpell1() { }

    // TODO
    public void UnequipSpell2() { }
    
    public void EquipPassiveSpell1(PassiveSpell spell)
    {
        equippedPassiveSpell1 = spell;
        spell.AddSpellAffects(passiveSpellAffects);
        spell.spawnTransform = _psm.gameObject.transform;
        spell.parentTransform = spellParentTransform;
        spell.SubscribeConditions(_psm);
    }

    public void EquipPassiveSpell2(PassiveSpell spell)
    {
        equippedPassiveSpell2 = spell;
        spell.AddSpellAffects(passiveSpellAffects);
        spell.spawnTransform = _psm.gameObject.transform;
        spell.parentTransform = spellParentTransform;
        spell.SubscribeConditions(_psm);
    }
    
    // TODO
    public void UnequipPassiveSpell1() { }

    // TODO
    public void UnequipPassiveSpell2() { }
}
