using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Lightable : MonoBehaviour
{
    [SerializeField] private Light2D light;
    [SerializeField] private bool isLit;
    
    public bool IsLit { get { return isLit; } }
    
    public delegate void LightActivated();
    public event LightActivated OnLightActivated;
    
    public delegate void LightDeactivated();
    public event LightDeactivated OnLightDeactivated;

    protected void Start()
    {
        if (!isLit)
        {
            light.enabled = false;
        }
    }

    protected void Update()
    {
        if (isLit && !light.enabled)
        {
            ActivateLightSource();
        } 
        else if (!isLit && light.enabled)
        {
            DeactivateLightSource();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Lightable>(out Lightable lightable))
        {
            if (isLit && lightable.isLit) return;
            
            if (!isLit && lightable.IsLit)
            {
                ActivateLightSource();
            }
            else if (isLit && !lightable.IsLit)
            {
                lightable.ActivateLightSource();
            }
        }
    }

    public void ActivateLightSource()
    {
        isLit = true;
        light.enabled = true;
        OnLightActivated?.Invoke();
    }

    public void DeactivateLightSource()
    {
        isLit = false;
        light.enabled = false;
        OnLightDeactivated?.Invoke();
    }
}
