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
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private Image spellItemImage;
    [SerializeField] private List<GameObject> spellGameObjects;
    
    // Consumables
    public int[] consumables = new int[2];
    private ConsumableTypes _equippedConsumable = ConsumableTypes.Bomb;
    
    // Spells
    public List<Spell> spells = new List<Spell>();
    
    // Getters & Setters
    public ConsumableTypes EquippedConsumable => _equippedConsumable;

    // Singleton instance
    public static InventoryManager Instance { get; private set; }

    // Events
    // - consumables
    public delegate void ConsumableSwitched(ConsumableTypes consumable);
    public event ConsumableSwitched OnConsumableSwitched;
    public delegate void ConsumableCountUpdated(int count, ConsumableTypes type);
    public event ConsumableCountUpdated OnConsumableCountUpdated;
    // - spells
    public delegate void SpellAdded(Spell spell);
    public event SpellAdded OnSpellAdded;
    public delegate void Spell1Equipped(SpellTypes spell);
    public event Spell1Equipped OnSpell1Equipped;
    public delegate void Spell2Equipped(SpellTypes spell);
    public event Spell2Equipped OnSpell2Equipped;

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
        inventoryUI.SetActive(false);

        foreach (GameObject g in spellGameObjects)
        {
            GameObject inst = Instantiate(g);
            if (inst.TryGetComponent<Spell>(out Spell spell))
            {
                AddSpell(spell);
            }
        }
    }

    public void OnInventoryPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;
        
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
    
    public void OnSwitchConsumable(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;

        _equippedConsumable = _equippedConsumable == ConsumableTypes.Bomb ? ConsumableTypes.BranchTorch : ConsumableTypes.Bomb;
        Debug.Log(_equippedConsumable);
        OnConsumableSwitched?.Invoke(_equippedConsumable);
    }

    private int GetConsumableIndex(ConsumableTypes type)
    {
        int index = (int)type;
        if (index == 0) return 0;
        if (index == 1) return 1;
        else if (index > 1)
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

    public void AddSpell(Spell spell)
    {
        spells.Add(spell);
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
}
