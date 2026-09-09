using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpellDictionary
{
    private readonly Dictionary<string, SpellConfig> _configs = new Dictionary<string, SpellConfig>();
    
    public SpellDictionary()
    {
        _configs["Wind Lord's Blessing"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/BounceSpellConfig");
        _configs["Cold Snap"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/ColdSnapSpellConfig");
        _configs["Dragon's Fury"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/FireballSpellConfig");
        _configs["Light Spell"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/LightSpellConfig");
        _configs["Reverse Footsteps"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/ReverseFootstepsSpellConfig");
        _configs["Snap Speed"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/SnapSpeedSpellConfig");
        _configs["World Aflame"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/WorldAflameSpellConfig");
        _configs["Devour"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/DevourSpellConfig");
        _configs["Dodge"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/DodgeSpellConfig");
        _configs["Feathery Dive"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/FeatheryDiveSpellConfig");
        _configs["Light Spear"] = Resources.Load<SpellConfig>("Data/Spells/SpellConfigs/LightSpearSpellConfig");
    }

    public SpellConfig GetConfig(string configName)
    {
        return _configs[configName];
    }
}
