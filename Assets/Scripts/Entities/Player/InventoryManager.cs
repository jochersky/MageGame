using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableTypes
{
    Bomb,
    BranchTorch
} 

public class InventoryManager : MonoBehaviour
{
    public int[] consumables = new int[2];

    // singleton instance
    public static InventoryManager Instance { get; private set; }

    public delegate void ConsumableCountUpdated(int count, ConsumableTypes type);
    public event ConsumableCountUpdated OnConsumableCountUpdated;

    private void Awake()
    {
        // ensure only one instance of the inventory exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
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
        
    }
}
