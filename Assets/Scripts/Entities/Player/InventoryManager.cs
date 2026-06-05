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
    // Consumables
    private ConsumableManager _consumableManager;
    // public int[] consumables = new int[2];
    // private ConsumableTypes _equippedConsumable = ConsumableTypes.Bomb;
    public List<ConsumableConfig> consumableConfigs = new List<ConsumableConfig>();
    public Dictionary<GameObject, ConsumableConfig> consumableListItemInstances = new Dictionary<GameObject, ConsumableConfig>();
    [HideInInspector] public int consumableToEquip = 0;
    
    // Spells
    private SpellManager _spellManager;
    public List<SpellConfig> spells = new List<SpellConfig>();
    public Dictionary<GameObject, SpellConfig> spellListItemInstances = new Dictionary<GameObject, SpellConfig>();
    [HideInInspector] public int spellToEquip = 0;
    
    // Singleton instance
    public static InventoryManager Instance { get; private set; }

    // Events
    // - Consumables
    public delegate void ConsumableSwitched(ConsumableConfig consumableConfig, int count);
    public event ConsumableSwitched OnConsumableSwitched;
    public delegate void ConsumableCountUpdated(ConsumableConfig consumableConfig, int count);
    public event ConsumableCountUpdated OnConsumableCountUpdated;
    
    public delegate void ConsumableAdded(ConsumableConfig consumable);
    public event ConsumableAdded OnConsumableAdded;
    public delegate void Consumable1Equipped(int equipSlot, ConsumableConfig consumableConfig, int count, bool visible);
    public event Consumable1Equipped OnConsumable1Equipped;
    public delegate void Consumable2Equipped(int equipSlot, ConsumableConfig consumableConfig, int count, bool visible);
    public event Consumable2Equipped OnConsumable2Equipped;
    public delegate void ConsumableUnequipped(int consumableID);
    public event ConsumableUnequipped OnConsumableUnequipped;
    // - Spells
    public delegate void SpellAdded(SpellConfig spellConfig);
    public event SpellAdded OnSpellAdded;
    public delegate void Spell1Equipped(Sprite spellSprite, bool visible);
    public event Spell1Equipped OnSpell1Equipped;
    public delegate void Spell2Equipped(Sprite spellSprite, bool visible);
    public event Spell2Equipped OnSpell2Equipped;
    public delegate void Spell1Unequipped(Sprite spellSprite, bool visible);
    public event Spell1Unequipped OnSpell1Unequipped;
    public delegate void Spell2Unequipped(Sprite spellSprite, bool visible);
    public event Spell2Unequipped OnSpell2Unequipped;
    
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
        _spellManager = GetComponent<SpellManager>();
        _consumableManager = GetComponent<ConsumableManager>();
    }
    
    public int GetConsumableCount(ConsumableConfig config)
    {
        return _consumableManager.GetConsumableCount(config);
    }
    
    public void UpdateConsumableCount(ConsumableConfig config, int amount)
    {
        OnConsumableCountUpdated?.Invoke(config, amount);
    }

    public void UpdateConsumableEquipped(ConsumableConfig config, int amount)
    {
        OnConsumableSwitched?.Invoke(config, amount);
    }

    public void OnNewConsumableAdded(ConsumableConfig consumableConfig)
    {
        OnConsumableAdded?.Invoke(consumableConfig);
    }

    public void AddConsumableListItem(GameObject consumableListItemGO, ConsumableConfig consumableConfig)
    {
        consumableListItemInstances.Add(consumableListItemGO, consumableConfig);
    }

    public void AddSpellListItem(GameObject spellListItemGO, SpellConfig config)
    {
        spellListItemInstances.Add(spellListItemGO, config);
    }

    public bool EquipSpell(GameObject spellListItemGO)
    {
        SpellConfig config = spellListItemInstances[spellListItemGO];
        
        SpellConfig spellConfig1 = _spellManager.spellConfig1;
        SpellConfig spellConfig2 = _spellManager.spellConfig2;
        switch (spellToEquip)
        {
            case 1:
                // Already equipped spells swap slots
                if (spellConfig2 == config)
                {
                    _spellManager.EquipSpell1(spellConfig2);
                    OnSpell1Equipped?.Invoke(spellConfig2.icon, true);
                    _spellManager.EquipSpell2(spellConfig1);
                    OnSpell2Equipped?.Invoke(spellConfig1.icon, true);
                }
                // normal case
                else
                {
                    _spellManager.EquipSpell1(config);
                    OnSpell1Equipped?.Invoke(config.icon, true); 
                }
                return true;
            case 2: 
                // Already equipped spells swap slots
                if (spellConfig1 == config)
                {
                    _spellManager.EquipSpell2(spellConfig1);
                    OnSpell2Equipped?.Invoke(spellConfig1.icon, true);
                    _spellManager.EquipSpell1(spellConfig2);
                    OnSpell1Equipped?.Invoke(spellConfig2.icon, true);
                }
                // normal case
                else
                {
                    _spellManager.EquipSpell2(config);
                    OnSpell2Equipped?.Invoke(config.icon, true); 
                }
                return true;
        }
        
        return false;
    }

    public void EquipConsumableToSlot1(ConsumableConfig consumableConfig)
    {
        OnConsumable1Equipped?.Invoke(1, consumableConfig, _consumableManager.GetConsumableCount(consumableConfig), true);
    }
    
    public void EquipConsumableToSlot2(ConsumableConfig consumableConfig)
    {
        OnConsumable1Equipped?.Invoke(2, consumableConfig, _consumableManager.GetConsumableCount(consumableConfig), true);
    }

    public bool EquipConsumable(GameObject consumableListItemGO)
    {
        ConsumableConfig config = consumableListItemInstances[consumableListItemGO];
        
        ConsumableConfig consumableConfig1 = _consumableManager.consumableConfig1;
        ConsumableConfig consumableConfig2 = _consumableManager.consumableConfig2;
        int count1 = _consumableManager.GetConsumableCount(consumableConfig1);
        int count2 = _consumableManager.GetConsumableCount(consumableConfig2);
        bool visible1 = consumableConfig1;
        bool visible2 = consumableConfig2;
        switch (consumableToEquip)
        {
            case 1:
                // Already equipped consumables swap slots
                if (consumableConfig2 == config)
                {
                    _consumableManager.EquipConsumable1(consumableConfig2);
                    OnConsumable1Equipped?.Invoke(1, consumableConfig2, count2, visible2);
                    _consumableManager.EquipConsumable2(consumableConfig1);
                    OnConsumable2Equipped?.Invoke(2, consumableConfig1, count1, visible1);
                }
                // normal case
                else
                {
                    _consumableManager.EquipConsumable1(config);
                    OnConsumable1Equipped?.Invoke(consumableToEquip, consumableConfig1, count1, visible1);
                }
                return true;
            case 2: 
                // Already equipped consumables swap slots
                if (consumableConfig1 == config)
                {
                    _consumableManager.EquipConsumable2(consumableConfig1);
                    OnConsumable2Equipped?.Invoke(2, consumableConfig1, count1, visible1);
                    _consumableManager.EquipConsumable1(consumableConfig2);
                    OnConsumable1Equipped?.Invoke(1, consumableConfig2, count2, visible2);
                }
                // normal case
                else
                {
                    _consumableManager.EquipConsumable2(config);
                    OnConsumable2Equipped?.Invoke(2, consumableConfig2, count2, visible2);
                }
                return true;
        }
        
        return false;
    }

    public void UnequipSpell(int spellID)
    {
        switch (spellID)
        {
            case 1: 
                _spellManager.UnequipSpell1(); 
                OnSpell1Unequipped?.Invoke(null, false);
                break;
            case 2: 
                _spellManager.UnequipSpell2();
                OnSpell2Unequipped?.Invoke(null, false);
                break;
        }
    }
    
    public void UnequipConsumable(int consumableID)
    {
        switch (consumableID)
        {
            case 0:
                _consumableManager.UnequipConsumable(consumableID);
                break;
        }
    }

    public void AddItem(ItemConfig itemConfig, int count)
    {
        if (itemConfig.itemType == ItemType.Spell)
        {
            SpellConfig spellConfig = itemConfig as SpellConfig;
            if (!spellConfig) return;
            int spellEquipped = _spellManager.AddSpell(spellConfig);
            switch (spellEquipped)
            {
                case 1: OnSpell1Equipped?.Invoke(spellConfig.icon, true); break;
                case 2: OnSpell2Equipped?.Invoke(spellConfig.icon, true); break;
            }
            
            OnSpellAdded?.Invoke(spellConfig);
        }
        else if (itemConfig.itemType == ItemType.Consumable)
        {
            ConsumableConfig consumableConfig = itemConfig as ConsumableConfig;
            if (!consumableConfig) return;
            // new selection available in consumable menu only when new consumable found
            int consumableEquipped = _consumableManager.AddConsumable(consumableConfig, count);
            switch (consumableEquipped)
            {
                case 1: OnConsumable1Equipped?.Invoke(consumableEquipped, consumableConfig, _consumableManager.GetConsumableCount(consumableConfig), true); break;
                case 2: OnConsumable2Equipped?.Invoke(consumableEquipped, consumableConfig, _consumableManager.GetConsumableCount(consumableConfig), true); break;
            }
        }
    }
}
