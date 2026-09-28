using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "RopeStrategy", menuName = "Consumable Strategies/RopeStrategy")]
public class RopeStrategy : PlaceableConsumableStrategy
{
    public LayerMask environmentLayer;
    public LayerMask interactLayer;
    public float maxHeight = 5f;
    public float yMaxMargin = 0.5f;
    public float yMinMargin = 0.75f;
    public float overlapWidth = 2f;
    public Sprite topSprite;
    public Sprite topEndSprite;
    public Sprite midSprite;
    public Sprite botSprite;

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
        
        // Place rope where hit occurs
        
        // Check distance above player
        RaycastHit2D upperHit = Physics2D.Raycast(spawnPosition, Vector2.up, maxHeight, environmentLayer);
        float upperDistance = upperHit.distance != 0 ? upperHit.distance : maxHeight;

        // Check distance below player
        Vector2 upperCheckVector = new Vector2(spawnPosition.x, spawnPosition.y + upperDistance - 0.05f); // offset to avoid hitting ceiling
        RaycastHit2D lowerHit = Physics2D.Raycast(upperCheckVector, Vector2.down, maxHeight, environmentLayer);
        float lowerDistance = lowerHit.distance != 0 ? lowerHit.distance : maxHeight;

        // distance + 1, cutoff at next closest int
        int numRope = (int) lowerDistance + 1;

        // -0.5f accounts for middle of a tile
        Vector2 placementVec = new Vector2(spawnPosition.x, spawnPosition.y + upperDistance - 0.5f);
        GameObject inst;
        for (int i = 0; i < numRope; i++)
        {
            // first rope must always be top rope sprite
            if (i == 0)
            {
                inst = SpawnRope(spawnTransform, placementVec, topSprite);
                
                // collision only necessary for top rope, adjust the bounds
                BoxCollider2D boxCollider = inst.GetComponent<BoxCollider2D>();
                boxCollider.offset = new Vector2(boxCollider.offset.x, (float)-numRope / 2 + 0.5f);
                boxCollider.size = new Vector2(boxCollider.size.x, numRope - 0.5f); // offset to provide end padding 
                
                SetRopeMinMaxHeight(inst, spawnPosition.y - lowerDistance + 0.25f, spawnPosition.y + upperDistance - 0.25f);
            }
            // last rope must always be bot rope sprite
            else if (i == numRope - 1)
            {
                inst = SpawnRope(spawnTransform, placementVec, botSprite);
                inst.GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                inst = SpawnRope(spawnTransform, placementVec, midSprite);
                inst.GetComponent<BoxCollider2D>().enabled = false;
            }
            
            // displace rope tiles by 1 unit
            placementVec.y -= 1f;
        }
        
        if (debug)
        {
            Debug.DrawRay(spawnPosition, Vector2.up * upperDistance, Color.green, 10f); // upper hit
            Debug.DrawRay(new Vector3(upperCheckVector.x + 0.1f, upperCheckVector.y), Vector2.down * lowerDistance, Color.red, 10f); // lower hit
        }

        return false;
    }

    private void SetRopeMinMaxHeight(GameObject inst, float yMin, float yMax)
    {
        Rope rope = inst.GetComponent<Rope>();
        rope.yMin = yMin;
        rope.yMax = yMax;
    }
    
    private GameObject SpawnRope(Transform spawnTransform, Vector3 spawnPosition, Sprite sprite)
    {
        // using spawn transform lets consumable be flipped
        GameObject inst = Instantiate(prefab, spawnTransform);
        
        float snappedX = Mathf.RoundToInt(spawnPosition.x) + 0.5f;
        // float snappedY = Mathf.RoundToInt(spawnPosition.y) - 0.5f;
        Vector3 snappedToGrid = new Vector3(snappedX, spawnPosition.y, spawnPosition.z);
        inst.transform.position = snappedToGrid;
        // null so that it won't follow the player's movement 
        inst.transform.parent = null;
        
        inst.GetComponent<SpriteRenderer>().sprite = sprite;
        
        return inst;
    }
}
