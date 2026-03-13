using UnityEngine;
using UnityEngine.InputSystem;

public enum SpellTypes
{
    GiftOfLight,
    WindLordsBlessing,
    FuryOfTheDragon
}

[RequireComponent(typeof(PlayerStateMachine))]
public class SpellManager1 : MonoBehaviour
{
    [SerializeField] private Transform spellCastTransform;
    [SerializeField] private Transform spellParentTransform;
    private PlayerStateMachine _psm;
    public Spell equippedSpell1;
    public Spell equippedSpell2;

    public static SpellManager1 Instance { get; private set; }
    
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
    }
    
    public void OnSpell1Pressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || _psm.IsDead) return;
        if (!equippedSpell1) return;
        
        equippedSpell1.CastSpell();
    }
    
    public void OnSpell2Pressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled || _psm.IsDead) return;
        if (!equippedSpell2) return;
        
        equippedSpell2.CastSpell();
    }

    public void EquipSpell1(Spell spell)
    {
        equippedSpell1 = spell;
        spell.spawnTransform = spellCastTransform;
        spell.parentTransform = spellParentTransform;
    }
    
    public void EquipSpell2(Spell spell)
    {
        equippedSpell2 = spell;
        spell.spawnTransform = spellCastTransform;
        spell.parentTransform = spellParentTransform;
    }
}
