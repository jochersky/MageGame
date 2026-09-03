using UnityEngine;
using UnityEngine.Tilemaps;

public class LandSharkInfo : EnemyInfo
{
    public override bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(tileCoords);
        bool below = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y - 1));
        bool left = colliderMap.HasTile(new Vector3Int(tileCoords.x + 1, tileCoords.y));
        return !self && (below || left);
    }
}
