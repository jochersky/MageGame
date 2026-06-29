using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpellDictionary
{
    private readonly Dictionary<string, SpellConfig> _configs = new Dictionary<string, SpellConfig>();
    
    public SpellDictionary()
    {
        _configs["Wind Lord's Blessing"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/BounceSpellConfig.asset");
        _configs["Cold Snap"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/ColdSnapSpellConfig.asset");
        _configs["Dragon's Fury"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/FireballSpellConfig.asset");
        _configs["Light Spell"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/LightSpellConfig.asset");
        _configs["Reverse Footsteps"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/ReverseFootstepsSpellConfig.asset");
        _configs["Snap Speed"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/SnapSpeedSpellConfig.asset");
        _configs["World Aflame"] = AssetDatabase.LoadAssetAtPath<SpellConfig>("Assets/Data/Spells/SpellConfigs/WorldAflameSpellConfig.asset");
    }

    public SpellConfig GetConfig(string configName)
    {
        return _configs[configName];
    }
}
