using UnityEngine;
using UnityEngine.Tilemaps;

public class DancingPlantInfo : EnemyInfo
{
    public override bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool left = colliderMap.HasTile(new Vector3Int(tileCoords.x - 1,  tileCoords.y));
        bool middle = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y));
        bool nonColliderMiddle = nonColliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y));
        bool right = colliderMap.HasTile(new Vector3Int(tileCoords.x + 1,  tileCoords.y));
        bool bottom = colliderMap.HasTile(new Vector3Int(tileCoords.x,  tileCoords.y + 1));
        return !left && !nonColliderMiddle && !middle && !right && bottom;
    }

}
