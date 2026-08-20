using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FullScreenEffect : MonoBehaviour
{
    private Image _image;
    private Material _material;
    
    private static readonly int VignetteIntensity = Shader.PropertyToID("_VignetteIntensity");

    private float _initialIntensity;
    private Coroutine _effectCoroutine;
    
    private void Start()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
        
        _material = _image.material;
        _initialIntensity = _material.GetFloat(VignetteIntensity);
    }

    public void StartScreenEffect(Color color, float duration)
    {
        if (_effectCoroutine != null) StopCoroutine(_effectCoroutine);
        
        _effectCoroutine = StartCoroutine(ScreenEffect(color, duration));
    }

    private IEnumerator ScreenEffect(Color color, float duration)
    {
        _image.enabled = true;
        _material.SetFloat(VignetteIntensity, _initialIntensity);
        _image.color = color;

        float timer = 0;
        while (timer < duration)
        {
            float newIntensity = _material.GetFloat(VignetteIntensity) * (1 - timer / duration);
            _material.SetFloat(VignetteIntensity, newIntensity);
            timer += Time.deltaTime;
            yield return null;
        }
        
        _image.enabled = false;
    }
}
