using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSpellAffects", menuName = "Scriptable Objects/PassiveSpellAffects")]
public class PassiveSpellAffects : ScriptableObject
{
    [Header("Jump Effects")]
    public int doubleJumps = 0;

    public void ClearAffects()
    {
        doubleJumps = 0;
    }
}
