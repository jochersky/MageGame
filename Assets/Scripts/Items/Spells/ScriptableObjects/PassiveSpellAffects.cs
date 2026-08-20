using UnityEngine;

[CreateAssetMenu(fileName = "PassiveSpellAffects", menuName = "Scriptable Objects/PassiveSpellAffects")]
public class PassiveSpellAffects : ScriptableObject
{
    [Header("Jump Effects")]
    public int doubleJumps = 0;

    [Header("Dodge Effects")] 
    public int dodges = 0;
    
    [Header("Devour Effects")]
    public bool canDevour = false;
    
    [Header("Light Radius Effects")]
    private float _lightRadiusDiff;
    public float LightRadiusDiff {
        get => _lightRadiusDiff;
        set
        {
            _lightRadiusDiff = value;
            OnLightRadiusUpdated?.Invoke(_lightRadiusDiff);
        }
    }

    public delegate void LightRadiusUpdated(float newLightRadiusDiff);
    public event LightRadiusUpdated OnLightRadiusUpdated;

    public void ClearAffects()
    {
        doubleJumps = 0;
        if (GameManager.Instance.CharacterType != CharacterType.Hound) dodges = 0;
        canDevour = false;
        LightRadiusDiff = 0;
    }
}
