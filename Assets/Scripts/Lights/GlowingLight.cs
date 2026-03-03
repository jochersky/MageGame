using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GlowingLight : MonoBehaviour
{
    [SerializeField] private float dilation = 0.25f;
    [SerializeField] private float speed = 1;
    [SerializeField] private float offset;

    [SerializeField] private bool addSecondWave;
    [SerializeField] private float secondDilation = 0.25f;
    [SerializeField] private float secondSpeed = 1;
    [SerializeField] private float secondOffset;
    
    private float _timer;
    private float _initialScale;

    private void Awake()
    {
        _initialScale = transform.localScale.x;
    }
    
    private void Update()
    {
        _timer += Time.deltaTime;
        float firstWave = Mathf.Cos((_timer * speed + offset)) * dilation;
        float secondWave = Mathf.Cos(_timer * secondSpeed + secondOffset) * secondDilation;
        float updatedScale = addSecondWave ? firstWave * secondWave : firstWave;
        updatedScale += _initialScale;
        transform.localScale = new Vector3(updatedScale, updatedScale, updatedScale);
    }
}
