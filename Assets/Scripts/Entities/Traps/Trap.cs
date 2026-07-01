using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Trap : MonoBehaviour
{
    public TileBase trapTile;
    // Range: 0 to 1
    public float[] levelSpawnRates = new float[3];
    public List<(int x, int y)> spawnPositions = new();
    public bool isCollider = false;

    // Please ensure to check both the colliderMap and nonColliderMap. For example, chests are not colliders, but you don't want to overwrite them
    public abstract bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap);
}
