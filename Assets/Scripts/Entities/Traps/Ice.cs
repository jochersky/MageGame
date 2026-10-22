using UnityEngine;
using UnityEngine.Tilemaps;

public class Ice : Trap
{
    public override bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(tileCoords);
        bool above = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y + 1));
        return self && !above;
    }
}
