using UnityEngine;
using UnityEngine.Tilemaps;

public class SkeletonInfo : EnemyInfo
{
    public override bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y));
        bool below = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y + 1));
        return !self && below;
    }

    
}
