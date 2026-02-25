using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DamageFlash : MonoBehaviour
{
    [SerializeField] private AnimationCurve _flashSpeedCurve;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashTime = 0.25f;
    
    private SpriteRenderer _spriteRenderer;
    private Material _material;
    
    private Coroutine _flashCoroutine;
    
    // Material property hashes
    private static readonly int _flashColorHash = Shader.PropertyToID("_FlashColor");
    private static readonly int _flashAmountHash = Shader.PropertyToID("_FlashAmount");
    
    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
    }

    public void StartFlash()
    {
        _flashCoroutine = StartCoroutine(Flash());
    }

    private IEnumerator Flash()
    {
        _material.SetColor(_flashColorHash, flashColor);
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            float currentFlashAmount = Mathf.Lerp(1f, _flashSpeedCurve.Evaluate(timer), timer / flashTime);
            _material.SetFloat(_flashAmountHash, currentFlashAmount);
            yield return null;
        }
    }
}
