using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Damage Properties")]
    public int damageAmt;
    [SerializeField] private float cameraShakeAmt;
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float knockBackForce;
    [Header("Status Effect")]
    [SerializeField] private StatType statType;
    [SerializeField] private OperatorTypes operatorType;
    [SerializeField] private float effectAmt;
    [SerializeField] private float effectDuration;
    [SerializeField] private float effectDelay;

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
        _damageProperties.knockBackForce = knockBackForce;
        if (effectAmt > 0)
        {
            _damageProperties.effect = new StatusEffect()
            {
                statType = statType,
                operatorType = operatorType,
                amount = effectAmt,
                duration = effectDuration,
                delay = effectDelay
            };
        }
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
    // damage source -> hurt box
    public Vector2 direction;
    public float knockBackForce;
    public StatusEffect effect;
}

public struct StatusEffect
{
    public StatType statType;
    public OperatorTypes operatorType;
    public float amount;
    public float duration;
    public float delay;
}
