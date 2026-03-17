using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum ConsumableTypes
{
    Bomb,
    BranchTorch
}

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Image spellItemImage;
    [SerializeField] private List<GameObject> spellGameObjects;
    
    // Consumables
    public int[] consumables = new int[2];
    private ConsumableTypes _equippedConsumable = ConsumableTypes.Bomb;
    
    // Spells
    // - Active
    public List<ActiveSpell> activeSpells = new List<ActiveSpell>();
    public Dictionary<GameObject, ActiveSpell> ActiveSpellListItemInstances = new Dictionary<GameObject, ActiveSpell>();
    [HideInInspector] public int spellToEquip = 0;
    // - Passive
    public List<PassiveSpell> passiveSpells = new List<PassiveSpell>();
    public Dictionary<GameObject, PassiveSpell> PassiveSpellListItemInstances = new Dictionary<GameObject, PassiveSpell>();
    [HideInInspector] public int passiveSpellToEquip = 0;
    
    // Getters & Setters
    public ConsumableTypes EquippedConsumable => _equippedConsumable;

    // Singleton instance
    public static InventoryManager Instance { get; private set; }

    // Events
    // - Consumables
    public delegate void ConsumableSwitched(ConsumableTypes consumable);
    public event ConsumableSwitched OnConsumableSwitched;
    public delegate void ConsumableCountUpdated(int count, ConsumableTypes type);
    public event ConsumableCountUpdated OnConsumableCountUpdated;
    // - Spells
    public delegate void SpellAdded(ActiveSpell spell);
    public event SpellAdded OnSpellAdded;
    public delegate void PassiveSpellAdded(PassiveSpell spell);
    public event PassiveSpellAdded OnPassiveSpellAdded;
    
    public delegate void Spell1Equipped(SpellTypes spell);
    public event Spell1Equipped OnSpell1Equipped;
    public delegate void Spell2Equipped(SpellTypes spell);
    public event Spell2Equipped OnSpell2Equipped;
    
    public delegate void PassiveSpell1Equipped(SpellTypes spell);
    public event PassiveSpell1Equipped OnPassiveSpell1Equipped;
    public delegate void PassiveSpell2Equipped(SpellTypes spell);
    public event PassiveSpell2Equipped OnPassiveSpell2Equipped;

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
        // TODO: make sure this isn't required for anything
        // foreach (GameObject g in spellGameObjects)
        // {
        //     GameObject inst = Instantiate(g);
        //     if (inst.TryGetComponent<Spell>(out Spell spell))
        //     {
        //         AddSpell(spell);
        //     }
        // }
    }
    
    // Consumables
    
    public void OnSwitchConsumable(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;

        _equippedConsumable = _equippedConsumable == ConsumableTypes.Bomb ? ConsumableTypes.BranchTorch : ConsumableTypes.Bomb;
        OnConsumableSwitched?.Invoke(_equippedConsumable);
    }

    private int GetConsumableIndex(ConsumableTypes type)
    {
        int index = (int)type;
        if (index == 0) return 0;
        if (index == 1) return 1;
        if (index > 1)
        {
            index = (int)Mathf.Log(index, 2);
        }
            
        return index;
    }
    
    public int GetConsumableCount(ConsumableTypes type)
    {
        int index = GetConsumableIndex(type);
        return consumables[index]; 
    }
    
    public void UpdateConsumable(ConsumableTypes type, int amt)
    {
        int index = GetConsumableIndex(type);
        consumables[index] += amt;
        OnConsumableCountUpdated?.Invoke(consumables[index], type);
    }

    // Spells
    
    public void AddSpell(ActiveSpell spell)
    {
        activeSpells.Add(spell);
        OnSpellAdded?.Invoke(spell);
        
        if (!SpellManager1.Instance.equippedSpell1)
        {
            OnSpell1Equipped?.Invoke(spell.spellType);
            SpellManager1.Instance.EquipSpell1(spell);
        }
        else if (!SpellManager1.Instance.equippedSpell2)
        {
            OnSpell2Equipped?.Invoke(spell.spellType);
            SpellManager1.Instance.EquipSpell2(spell);
        }
    }

    public void AddPassiveSpell(PassiveSpell spell)
    {
        passiveSpells.Add(spell);
        OnPassiveSpellAdded?.Invoke(spell);
        
        if (!SpellManager1.Instance.equippedPassiveSpell1)
        {
            OnPassiveSpell1Equipped?.Invoke(spell.spellType);
            SpellManager1.Instance.EquipPassiveSpell1(spell);
        }
        else if (!SpellManager1.Instance.equippedPassiveSpell2)
        {
            OnPassiveSpell2Equipped?.Invoke(spell.spellType);
            SpellManager1.Instance.EquipPassiveSpell2(spell);
        }
    }

    public void AddSpellListItem(GameObject spellListItemGO, ActiveSpell spell)
    {
        ActiveSpellListItemInstances.Add(spellListItemGO, spell);
    }

    public void AddPassiveSpellListItem(GameObject spellListItemGO, PassiveSpell spell)
    {
        PassiveSpellListItemInstances.Add(spellListItemGO, spell);
    }

    public bool EquipActiveSpell(GameObject spellListItemGO)
    {
        ActiveSpell spell = ActiveSpellListItemInstances[spellListItemGO];
        
        switch (spellToEquip)
        {
            case 1:
                // Already equipped spells swap slots
                if (SpellManager1.Instance.equippedSpell2 == spell)
                {
                    ActiveSpell spell1 = SpellManager1.Instance.equippedSpell1;
                    OnSpell2Equipped?.Invoke(spell1.spellType);
                    SpellManager1.Instance.EquipSpell2(spell1);
                }
                    
                OnSpell1Equipped?.Invoke(spell.spellType); 
                SpellManager1.Instance.EquipSpell1(spell);
                return true;
            case 2: 
                // Already equipped spells swap slots
                if (SpellManager1.Instance.equippedSpell1 == spell)
                {
                    ActiveSpell spell2 = SpellManager1.Instance.equippedSpell2;
                    OnSpell1Equipped?.Invoke(spell2.spellType);
                    SpellManager1.Instance.EquipSpell1(spell2);
                }
                
                OnSpell2Equipped?.Invoke(spell.spellType); 
                SpellManager1.Instance.EquipSpell2(spell);
                return true;
        }
        
        return false;
    }
    
    public bool EquipPassiveSpell(GameObject spellListItemGO)
    {
        PassiveSpell spell = PassiveSpellListItemInstances[spellListItemGO];
        
        switch (passiveSpellToEquip)
        {
            case 1:
                // Already equipped spells swap slots
                if (SpellManager1.Instance.equippedPassiveSpell2 == spell)
                {
                    PassiveSpell spell1 = SpellManager1.Instance.equippedPassiveSpell1;
                    OnPassiveSpell2Equipped?.Invoke(spell1.spellType);
                    SpellManager1.Instance.EquipPassiveSpell2(spell1);
                }
                    
                OnPassiveSpell1Equipped?.Invoke(spell.spellType);
                SpellManager1.Instance.EquipPassiveSpell1(spell);
                return true;
            case 2: 
                // Already equipped spells swap slots
                if (SpellManager1.Instance.equippedPassiveSpell1 == spell)
                {
                    PassiveSpell spell2 = SpellManager1.Instance.equippedPassiveSpell2;
                    OnPassiveSpell1Equipped?.Invoke(spell2.spellType);
                    SpellManager1.Instance.EquipPassiveSpell1(spell2);
                }
                
                OnPassiveSpell2Equipped?.Invoke(spell.spellType);
                SpellManager1.Instance.EquipPassiveSpell2(spell);
                return true;
        }
        
        return false;
    }
}
