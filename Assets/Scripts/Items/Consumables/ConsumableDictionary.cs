using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConsumableDictionary
{
    private readonly Dictionary<string, ConsumableConfig> _configs = new Dictionary<string, ConsumableConfig>();
    
    public ConsumableDictionary()
    {
        _configs["Bomb"] = AssetDatabase.LoadAssetAtPath<ConsumableConfig>("Assets/Data/Consumables/Configs/BombConfig.asset");
        _configs["Drill Bomb"] = AssetDatabase.LoadAssetAtPath<ConsumableConfig>("Assets/Data/Consumables/Configs/DrillBombConfig.asset");
        _configs["Rope"] = AssetDatabase.LoadAssetAtPath<ConsumableConfig>("Assets/Data/Consumables/Configs/RopeConfig.asset");
        _configs["Sol Stone"] = AssetDatabase.LoadAssetAtPath<ConsumableConfig>("Assets/Data/Consumables/Configs/SolStoneConfig.asset");
    }

    public ConsumableConfig GetConfig(string configName)
    {
        return _configs[configName];
    }
}

