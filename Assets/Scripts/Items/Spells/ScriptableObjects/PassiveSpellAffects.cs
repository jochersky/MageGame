using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSpellAffects", menuName = "Scriptable Objects/PassiveSpellAffects")]
public class PassiveSpellAffects : ScriptableObject
{
    [Header("Jump Effects")]
    public int doubleJumps = 0;

    [Header("Dodge Effects")] 
    public int dodges = 0;

    public void ClearAffects()
    {
        doubleJumps = 0;
        if (GameManager.Instance.CharacterType != CharacterType.Hound) dodges = 0;
    }
}
