using UnityEngine;

[CreateAssetMenu(fileName = "SpellConfig", menuName = "ScriptableObjects/SpellConfig")]
public class SpellConfig : ItemConfig
{
    [Header("Spell Info")] 
    public int manaCost = 1;
    public float cooldown;
    public SpellStrategy strategy;
    public PassiveEffectsStrategy effectsStrategy;
}