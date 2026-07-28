using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChestManager : MonoBehaviour
{
    [SerializeField] string configPath;
    private List<SpellConfig> spells;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SpellConfig[] spellArray = Resources.LoadAll<SpellConfig>(configPath);
        spells = new List<SpellConfig>(spellArray);
    }

    public SpellConfig GetSpellConfig()
    {
        if (spells.Count <= 0)
        {
            throw new System.Exception("ChestManager has no spells available");
        }
        int randIdx = Random.Range(0, spells.Count);
        SpellConfig spell = spells[randIdx];
        spells.RemoveAt(randIdx);
        return spell;
    }
}
