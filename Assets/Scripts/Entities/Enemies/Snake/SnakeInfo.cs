using UnityEngine;
using UnityEngine.Tilemaps;

public class SnakeInfo : EnemyInfo
{
    public override bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.GetTile(tileCoords);
        return self;
    }

}
