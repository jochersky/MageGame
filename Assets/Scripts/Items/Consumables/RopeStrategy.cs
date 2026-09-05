using System.Collections.Generic;
using UnityEngine;

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
        
        RaycastHit2D hit = Physics2D.Raycast(spawnPosition, Vector2.up, maxHeight, environmentLayer);
        
        // max height rope
        if (hit.distance == 0)
        {
            // top
            Vector3 ropeTopP = Vector3.up * (Mathf.RoundToInt(spawnPosition.y + maxHeight) - 2f);
            GameObject inst = SpawnRope(spawnTransform, spawnPosition + ropeTopP, topSprite);
            GameObject topInst = inst;
            BoxCollider2D boxCollider = inst.GetComponent<BoxCollider2D>();
            boxCollider.offset = new Vector2(boxCollider.offset.x, -maxHeight / 2 + 0.5f);
            boxCollider.size = new Vector2(boxCollider.size.x, maxHeight);
            
            // middle
            for (int i = 1; i < maxHeight - 1; i++)
            {
                inst = SpawnRope(spawnTransform, spawnPosition + ropeTopP - (Vector3.up * i), midSprite);
                inst.GetComponent<BoxCollider2D>().enabled = false;
            }
            
            // end
            inst = SpawnRope(spawnTransform, spawnPosition, botSprite);
            inst.GetComponent<BoxCollider2D>().enabled = false;
            
            float ropeYPos = boxCollider.transform.position.y + boxCollider.offset.y;
            SetRopeMinMaxHeight(topInst, ropeYPos - (boxCollider.size.y / 2) + yMinMargin, ropeYPos + (boxCollider.size.y / 2) - yMaxMargin);
        }
        else
        {
            // single tall rope
            if (hit.distance < 1)
            {
                GameObject inst = SpawnRope(spawnTransform, spawnPosition, topEndSprite);
                SetRopeMinMaxHeight(inst, inst.transform.position.y - 0.5f, inst.transform.position.y + 0.5f);
            }
            // variable size rope
            else
            {
                // top
                Vector3 ropeTopP = Vector3.up * (hit.distance - 0.5f);
                GameObject inst = SpawnRope(spawnTransform, spawnPosition + ropeTopP, topSprite);
                GameObject topInst = inst;
                BoxCollider2D boxCollider = inst.GetComponent<BoxCollider2D>();
                boxCollider.offset = new Vector2(boxCollider.offset.x, -(hit.distance - 0.5f) / 2);
                boxCollider.size = new Vector2(boxCollider.size.x, hit.distance + 0.5f);
        
                // middle
                for (int i = 1; i < hit.distance - 0.5f; i++)
                {
                    inst = SpawnRope(spawnTransform, spawnPosition + ropeTopP - (Vector3.up * i), midSprite);
                    inst.GetComponent<BoxCollider2D>().enabled = false;
                }
            
                // end
                inst = SpawnRope(spawnTransform, spawnPosition, botSprite);
                inst.GetComponent<BoxCollider2D>().enabled = false;

                float ropeYPos = boxCollider.transform.position.y + boxCollider.offset.y;
                SetRopeMinMaxHeight(topInst, ropeYPos - (boxCollider.size.y / 2) + yMinMargin, ropeYPos + (boxCollider.size.y / 2) - yMaxMargin);
            }
        }

        if (debug)
        {
            Debug.DrawRay(spawnPosition, Vector2.up * maxHeight, Color.green, 10f);
            Debug.DrawRay(spawnPosition, Vector2.up * hit.distance, Color.red, 10f);
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
