using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Damage Properties")]
    public int damageAmt;
    [SerializeField] private float cameraShakeAmt;
    [SerializeField] private float cameraShakeDuration;

    [Header("Hitbox Colliders")] 
    [SerializeField] private Collider2D[] colliders;

    private DamageProperties _damageProperties;
    public DamageProperties DamageProperties => _damageProperties;
    
    private void Start()
    {
        _damageProperties.amount = damageAmt;
        _damageProperties.cameraShakeProperties = new CameraShakeProperties
        {
            amount = cameraShakeAmt,
            duration = cameraShakeDuration
        };
    }
    
    public void Disable()
    {
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
}

public struct DamageProperties
{
    public int amount;
    public CameraShakeProperties cameraShakeProperties;
} 
