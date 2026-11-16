using System.Collections;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer[] otherSprites;
    [SerializeField] private AnimationCurve _flashSpeedCurve;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;
    
    private Material _material;
    private Material[] _otherMaterials;
    
    private Coroutine _flashCoroutine;
    
    // Material property hashes
    private static readonly int _flashColorHash = Shader.PropertyToID("_FlashColor");
    private static readonly int _flashAmountHash = Shader.PropertyToID("_FlashAmount");

    public delegate void DamageFlashComplete();
    public event DamageFlashComplete OnDamageFlashComplete;
    
    private void Start()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        _material = spriteRenderer.material;

        _otherMaterials = new Material[otherSprites.Length];
        if (_otherMaterials.Length <= 0) return;
        
        // Get material references
        for (int i = 0; i < otherSprites.Length; i++)
            _otherMaterials[i] =  otherSprites[i].material;
    }

    public void StartFlash()
    {
        _flashCoroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        // Set color
        if (_material) _material.SetColor(_flashColorHash, flashColor);
        foreach (Material material in _otherMaterials)
            material.SetColor(_flashColorHash, flashColor);
        
        // Dissipate color over time
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            float currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(timer), timer / flashTime);
            
            if (_material) _material.SetFloat(_flashAmountHash, currentFlashAmount);
            foreach (Material material in _otherMaterials)
                material.SetFloat(_flashAmountHash, currentFlashAmount);
            
            yield return null;
        }
        OnDamageFlashComplete?.Invoke();
    }
}
