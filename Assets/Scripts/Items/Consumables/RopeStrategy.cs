using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "RopeStrategy", menuName = "Consumable Strategies/RopeStrategy")]
public class RopeStrategy : PlaceableConsumableStrategy
{
    // public LayerMask environmentLayer;
    public LayerMask interactLayer;
    // public float maxHeight = 5f;
    public float overlapWidth = 2f;
    // public Sprite topSprite;
    // public Sprite topEndSprite;
    // public Sprite midSprite;
    // public Sprite botSprite;

    public bool debug = true;

    private List<RaycastHit2D> _interactHits;
    private ContactFilter2D _contactFilter;
    
    public override bool UsePlaceableConsumable(Transform spawnTransform, Vector3 spawnPosition)
    {
        _interactHits ??= new List<RaycastHit2D>();
        
        _contactFilter = new ContactFilter2D
        {
            layerMask = interactLayer,
            useLayerMask = true,
            useTriggers = true,
        };

        // Check for duplicate ropes
        Vector2 adjustedSpawnLocation = new Vector2(spawnPosition.x + overlapWidth / 2, spawnPosition.y);
        Vector2 overlapDir = new Vector2(-overlapWidth, 0f);
        
        Physics2D.Raycast(adjustedSpawnLocation,overlapDir, _contactFilter, _interactHits, overlapWidth);

        if (debug) Debug.DrawRay(adjustedSpawnLocation,overlapDir, Color.teal, 5f);

        foreach (var iHit in _interactHits)
        {
            if (iHit.collider.gameObject.TryGetComponent<Rope>(out Rope rope))
            {
                if (debug) Debug.Log("rope already here");
                _interactHits.Clear();
                return false;
            }
        }
        
        _interactHits.Clear();

        GameObject inst = Instantiate(prefab, spawnTransform);
        inst.transform.position = spawnPosition;
        inst.transform.parent = null;

        return true;
    }
}
