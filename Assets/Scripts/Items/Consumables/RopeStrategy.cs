using UnityEngine;

[CreateAssetMenu(fileName = "RopeStrategy", menuName = "Consumable Strategies/RopeStrategy")]
public class RopeStrategy : PlaceableConsumableStrategy
{
    public LayerMask environmentLayer;
    public float maxHeight = 5f;
    public float yMaxMargin = 0.5f;
    public float yMinMargin = 0.75f;
    public Sprite topSprite;
    public Sprite topEndSprite;
    public Sprite midSprite;
    public Sprite botSprite;

    public bool debug = false;
    
    public override void UsePlaceableConsumable(Transform spawnTransform, Vector3 spawnPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(spawnPosition, Vector2.up, maxHeight, environmentLayer);
        
        // max height rope
        if (hit.distance == 0)
        {
            // top
            Vector3 ropeTopP = Vector3.up * (maxHeight - 1);
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
            Debug.DrawRay(spawnPosition, Vector2.up * maxHeight, Color.green, 3f);
            Debug.DrawRay(spawnPosition, Vector2.up * hit.distance, Color.red, 3f);
        }
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
        inst.transform.position = spawnPosition;
        // null so that it won't follow the player's movement 
        inst.transform.parent = null;
        
        inst.GetComponent<SpriteRenderer>().sprite = sprite;
        
        return inst;
    }
}
