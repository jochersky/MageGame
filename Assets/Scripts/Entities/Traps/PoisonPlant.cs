using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PoisonPlant : Trap
{
    [SerializeField] float interval = 2f;
    [SerializeField] PoisonBubble poisonBubble;
    [SerializeField] float spawnOffset = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnBubble), interval, interval);
    }

    // Update is called once per frame
    void SpawnBubble() 
    {
        Instantiate(poisonBubble, new Vector3(transform.position.x, transform.position.y + spawnOffset, transform.position.z), Quaternion.identity);
    }

    // currently, a poison plant can spawn only on a tile with at least two empty spaces above it and solid space below it
    public override bool checkIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderTilemap)
    {
        // We ignore the top three rows of tiles, as none can have three spaces above them
        if (tileCoords.y < -2)
            {
                Vector3Int below = new(tileCoords.x, tileCoords.y - 1);
                Vector3Int above = new(tileCoords.x, tileCoords.y + 1, 0);
                Vector3Int aboveAbove = new(above.x, above.y + 1, 0);
                Vector3Int aboveAboveAbove = new(aboveAbove.x, aboveAbove.y + 1, 0);
                TileBase tileBelow = colliderTilemap.GetTile(below);
                TileBase tileAbove = colliderTilemap.GetTile(above);
                TileBase tileAboveAbove = colliderTilemap.GetTile(aboveAbove);
                TileBase tileAboveAboveAbove = colliderTilemap.GetTile(aboveAboveAbove);
                if (tileBelow != null && currTile == null && tileAbove == null && tileAboveAbove == null && tileAboveAboveAbove == null)
                {
                    return true;
                }
            }
        return false;
    }
}
