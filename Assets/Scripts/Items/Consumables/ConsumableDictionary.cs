using System.Collections.Generic;
using UnityEngine;

public class ConsumableDictionary
{
    private readonly Dictionary<string, ConsumableConfig> _configs = new Dictionary<string, ConsumableConfig>();
    
    public ConsumableDictionary()
    {
        _configs["Bomb"] = Resources.Load<ConsumableConfig>("Data/Consumables/Configs/BombConfig");
        _configs["Drill Bomb"] = Resources.Load<ConsumableConfig>("Data/Consumables/Configs/DrillBombConfig");
        _configs["Rope"] = Resources.Load<ConsumableConfig>("Data/Consumables/Configs/RopeConfig");
        _configs["Sol Stone"] = Resources.Load<ConsumableConfig>("Data/Consumables/Configs/SolStoneConfig");
    }

    public ConsumableConfig GetConfig(string configName)
    {
        return _configs[configName];
    }
}

