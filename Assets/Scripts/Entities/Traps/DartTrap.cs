using UnityEngine;
using UnityEngine.Tilemaps;

public class DartTrap : Trap
{
    [SerializeField] Dart dartPrefab;
    public bool facingRight = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && (collision.CompareTag("Player") || collision.CompareTag("Enemy")))
        {
            if (facingRight)
            {
                dartPrefab.direction = transform.right;
            } else
            {
                dartPrefab.direction = -transform.right;
            }
            Instantiate(dartPrefab, transform.position, Quaternion.identity);
        } 
    }

    public override bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        if (facingRight)
        {
            // We ignore the rightmost and leftmost columns
            if (tileCoords.x < colliderMap.cellBounds.max.x && tileCoords.x > colliderMap.cellBounds.min.x)
            {
                bool self = colliderMap.HasTile(tileCoords) || nonColliderMap.HasTile(tileCoords);
                bool right = colliderMap.HasTile(new(tileCoords.x + 1, tileCoords.y));
                bool rightRight = colliderMap.HasTile(new(tileCoords.x + 2, tileCoords.y));
                //bool rightRightRight = colliderMap.HasTile(new(tileCoords.x + 3, tileCoords.y));
                bool left = colliderMap.HasTile(new(tileCoords.x - 1, tileCoords.y));
                if (!self && left && !right && !rightRight)
                {
                    return true;
                }
            }
        } else
        {
            // We ignore the rightmost and leftmost columns
            if (tileCoords.x < colliderMap.cellBounds.max.x && tileCoords.x > colliderMap.cellBounds.min.x)
            {
                bool self = colliderMap.HasTile(tileCoords) || nonColliderMap.HasTile(tileCoords);
                bool left = colliderMap.HasTile(new(tileCoords.x - 1, tileCoords.y));
                bool leftLeft = colliderMap.HasTile(new(tileCoords.x - 2, tileCoords.y));
                //bool leftLeftLeft = colliderMap.HasTile(new(tileCoords.x - 3, tileCoords.y));
                bool right = colliderMap.HasTile(new(tileCoords.x + 1, tileCoords.y));
                if (!self && right && !left && !leftLeft)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}
