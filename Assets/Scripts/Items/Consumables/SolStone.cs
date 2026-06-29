using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SolStone : MonoBehaviour
{
    [SerializeField] private Color[] colors;
    [SerializeField] private SpriteRenderer stoneSprite;
    [SerializeField] private Light2D light2D;
    
    private void Start()
    {
        if (colors.Length <= 0) return;

        Color color = colors[Random.Range(0, colors.Length)];
        stoneSprite.color = color;
        light2D.color = color;
    }
}
