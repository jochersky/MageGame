using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableTypes
{
    Bomb,
} 

public class Inventory : MonoBehaviour
{
    public int[] consumables = new int[1];

    // singleton instance
    public static Inventory Instance { get; private set; }

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
        if (index > 0)
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
    }

    public void AddSpell(Spell spell)
    {
        
    }
}
