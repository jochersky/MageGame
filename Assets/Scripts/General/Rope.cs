using UnityEngine;

/*
 * Class that contains creation of ropes.
 * 
 * Rope transform parenting structure:
 * 
 *   Top Rope (contains logic + collsion box)
 *      ...
 *      Middle Ropes (just sprite)
 *      ...
 *      Bottom Rope (just sprite)
 */
public class Rope : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private LayerMask environmentLayer;
    [Header("References")]
    [SerializeField] private BoxCollider2D collider;
    [SerializeField] private GameObject ropeSpritePrefab;
    [Header("Sprites")]
    [SerializeField] private Sprite topSprite;
    [SerializeField] private Sprite topEndSprite;
    [SerializeField] private Sprite midSprite;
    [SerializeField] private Sprite botSprite;
    public float yMin;
    public float yMax;
    [SerializeField] private bool debug = false;

    private void Start()
    {
        float snappedX = Mathf.RoundToInt(transform.position.x) + 0.5f;
        Vector3 snappedToGrid = new Vector3(snappedX, transform.position.y, transform.position.z);
        transform.position = snappedToGrid;
        // null so that it won't follow the player's movement 
        transform.parent = null;

        GrowRope();
    }

    private void GrowRope()
    {
        // Check distance above spawn
        RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Vector2.up, maxHeight, environmentLayer);
        float upperDistance = upperHit.distance != 0 ? upperHit.distance : maxHeight;

        // Check distance below spawn
        Vector2 upperCheckVector = new Vector2(transform.position.x, transform.position.y + upperDistance - 0.05f); // offset to avoid hitting ceiling
        RaycastHit2D lowerHit = Physics2D.Raycast(upperCheckVector, Vector2.down, maxHeight, environmentLayer);
        float lowerDistance = lowerHit.distance != 0 ? lowerHit.distance : maxHeight;
        
        // distance + 1, cutoff at next closest int
        int numRope = (int) lowerDistance + 1;

        // -0.5f accounts for middle of a tile
        Vector2 placementVec = new Vector2(transform.position.x, transform.position.y + upperDistance - 0.5f);
        for (int i = 0; i < numRope; i++)
        {
            // first rope must always be top rope sprite
            if (i == 0)
            {
                transform.position = placementVec;
                
                // collision only necessary for top rope, adjust the bounds
                collider.offset = new Vector2(collider.offset.x, (float)-numRope / 2 + 0.5f);
                collider.size = new Vector2(collider.size.x, numRope - 0.5f); // offset to provide end padding 
                
                yMin = transform.position.y - lowerDistance + 0.25f;
                yMax = transform.position.y + upperDistance - 0.25f;
            }
            // last rope must always be bot rope sprite
            else if (i == numRope - 1)
            {
                SpawnRopeTile(placementVec, botSprite);
            }
            else
            {
                SpawnRopeTile(placementVec, midSprite);
            }
            
            // displace rope tiles by 1 unit
            placementVec.y -= 1f;
        }
        
        if (debug)
        {
            Debug.DrawRay(transform.position, Vector2.up * upperDistance, Color.green, 10f); // upper hit
            Debug.DrawRay(new Vector3(upperCheckVector.x + 0.1f, upperCheckVector.y), Vector2.down * lowerDistance, Color.red, 10f); // lower hit
        }
    }

    private void SpawnRopeTile(Vector3 spawnPosition, Sprite sprite)
    {
        GameObject inst = Instantiate(ropeSpritePrefab, transform);
        
        // Round position of rope tile
        float snappedX = Mathf.RoundToInt(spawnPosition.x) + 0.5f;
        Vector3 snappedToGrid = new Vector3(snappedX, spawnPosition.y, spawnPosition.z);
        inst.transform.position = snappedToGrid;
        
        // Adjust sprite to its height in the rope
        inst.GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
