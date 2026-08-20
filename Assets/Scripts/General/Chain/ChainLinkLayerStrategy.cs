using UnityEngine;

[CreateAssetMenu(fileName = "ChainLinkLayerStrategy", menuName = "Chain Link Strategies/ChainLinkLayerStrategy")]
public class ChainLinkLayerStrategy : ScriptableObject
{
    public uint frontLayer;
    public uint backLayer;
    public float goToFrontPercentage;
    public float goToBackPercentage;

    public void UpdateSpriteLayer(SpriteRenderer spriteRenderer, float progress)
    {
        if (spriteRenderer.renderingLayerMask > backLayer && progress >= goToBackPercentage)
        {
            spriteRenderer.renderingLayerMask = backLayer;
        }
        else if (spriteRenderer.renderingLayerMask < frontLayer && progress >= goToFrontPercentage)
        {
            spriteRenderer.renderingLayerMask = frontLayer;
        }
    }
}
