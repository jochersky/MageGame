using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class LightSpell : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Light2D lightAura;
    float red;
    float green;
    float blue;
    float alpha;
    [SerializeField] float startingIntensity = 5f;
    // time between fade steps
    [SerializeField] float fadeRate = 1f;
    // How much alpha to reduce by per fade step
    [SerializeField] float spriteFadeStep = 0.1f;
    // how much intensity to reduce by per fade step
    [SerializeField] float brightnessFadeStep = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightAura = GetComponentInChildren<Light2D>();
        lightAura.intensity = startingIntensity;
        red = spriteRenderer.color.r;
        green = spriteRenderer.color.g;
        blue = spriteRenderer.color.b;
        alpha = spriteRenderer.color.a;
        InvokeRepeating(nameof(Fade), fadeRate, fadeRate);
    }

    void Fade()
    {
        alpha -= spriteFadeStep;
        spriteRenderer.color = new Color(red, green, blue, alpha);
        lightAura.intensity -= brightnessFadeStep;
    }

    // Update is called once per frame
    void Update()
    {
        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
