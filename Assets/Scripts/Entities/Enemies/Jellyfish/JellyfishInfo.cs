using UnityEngine;
using UnityEngine.Tilemaps;

public class JellyfishInfo : EnemyInfo
{
    public override bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool top = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y - 1));
        bool left = colliderMap.HasTile(new Vector3Int(tileCoords.x - 1,  tileCoords.y));
        bool middle = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y));
        bool right = colliderMap.HasTile(new Vector3Int(tileCoords.x + 1,  tileCoords.y));
        bool bottom = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y + 1));
        return !( top || left || middle || right || bottom);
    }
}
