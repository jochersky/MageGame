using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Bounce : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    float red;
    float green;
    float blue;
    float alpha;
    // time between fade steps
    [SerializeField] float fadeRate = 0.1f;
    // How much alpha to reduce by per fade step
    [SerializeField] float fadeStep = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        red = spriteRenderer.color.r;
        green = spriteRenderer.color.g;
        blue = spriteRenderer.color.b;
        alpha = spriteRenderer.color.a;
        InvokeRepeating(nameof(Fade), fadeRate, fadeRate);
    }

    void Fade()
    {
        Debug.Log(alpha);
        alpha -= fadeStep;
        spriteRenderer.color = new Color(red, green, blue, alpha);
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
