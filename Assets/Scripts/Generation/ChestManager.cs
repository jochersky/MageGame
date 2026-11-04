using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    [SerializeField] string configPath;
    [SerializeField] string consumableConfigPath;
    private List<SpellConfig> allSpells;
    private List<ConsumableConfig> allConsumables;
    private List<SpellConfig> availableSpells;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SpellConfig[] spellArray = Resources.LoadAll<SpellConfig>(configPath);
        allConsumables = new List<ConsumableConfig>(Resources.LoadAll<ConsumableConfig>(consumableConfigPath));
        allSpells = new List<SpellConfig>(spellArray);
        availableSpells = allSpells;
    }

    void CheckPlayerInventory()
    {
        InventoryManager inventory = FindAnyObjectByType<InventoryManager>();
        if (inventory)
            availableSpells = inventory.spells;
        else 
            Debug.Log("No inventory found.");
    }

    // TODO: Clean up
    public ItemConfig GetSpellConfig()
    {
        CheckPlayerInventory();
        List<SpellConfig> currentlyUnusedSpells = availableSpells;
        if (currentlyUnusedSpells.Count <= 0)
        {
            // player has most or all of the spells, so we give items instead
            return GetConsConfig();
        } else
        {
            int randIdx = Random.Range(0, currentlyUnusedSpells.Count);
            SpellConfig spell = currentlyUnusedSpells[randIdx];
            availableSpells.RemoveAt(randIdx);
            return spell;
        }
    }

    public ConsumableConfig GetConsConfig()
    {
        int randIdx = Random.Range(0, allConsumables.Count);
        ConsumableConfig cons = allConsumables[randIdx];
        return cons;
    }
}
