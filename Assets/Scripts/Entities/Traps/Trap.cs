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

    public abstract bool checkIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap);
}
