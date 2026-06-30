using UnityEngine;
using UnityEngine.Tilemaps;

public class SpiderInfo : EnemyInfo
{
    public override bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool topLeft = colliderMap.HasTile(new Vector3Int(tileCoords.x - 1,  tileCoords.y - 1));
        bool top = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y - 1));
        bool topRight = colliderMap.HasTile(new Vector3Int(tileCoords.x + 1,  tileCoords.y - 1));
        bool left = colliderMap.HasTile(new Vector3Int(tileCoords.x - 1,  tileCoords.y));
        bool middle = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y));
        bool right = colliderMap.HasTile(new Vector3Int(tileCoords.x + 1,  tileCoords.y));
        bool bottomLeft = colliderMap.HasTile(new Vector3Int(tileCoords.x - 1,  tileCoords.y + 1));
        bool bottom = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y + 1));
        bool bottomRight = colliderMap.HasTile(new Vector3Int(tileCoords.x + 1,  tileCoords.y + 1));
        return !(topLeft || top || topRight || left || middle || right || bottomLeft || bottom || bottomRight);
    }
}
