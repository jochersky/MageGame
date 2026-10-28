using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class Lightable : MonoBehaviour
{
    [FormerlySerializedAs("light")] [SerializeField] private Light2D lighting;
    [SerializeField] private bool isLit;
    
    public virtual bool IsLit { get { return isLit; } }
    
    public delegate void LightActivated();
    public event LightActivated OnLightActivated;
    
    public delegate void LightDeactivated();
    public event LightDeactivated OnLightDeactivated;

    protected void Start()
    {
        if (!isLit)
        {
            lighting.enabled = false;
        }
    }

    protected void Update()
    {
        if (isLit && !lighting.enabled)
        {
            ActivateLightSource();
        } 
        else if (!isLit && lighting.enabled)
        {
            DeactivateLightSource();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Lightable>(out Lightable lightable))
        {
            if (lightable.CompareTag("Player") || isLit && lightable.isLit) return;
            
            if (!isLit && lightable.IsLit) 
                ActivateLightSource();
            else if (isLit && !lightable.IsLit) 
                lightable.ActivateLightSource();
        }
    }

    public void ActivateLightSource()
    {
        isLit = true;
        lighting.enabled = true;
        OnLightActivated?.Invoke();
    }

    public void DeactivateLightSource()
    {
        isLit = false;
        lighting.enabled = false;
        OnLightDeactivated?.Invoke();
    }
}
