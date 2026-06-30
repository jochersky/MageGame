using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class EnemyInfo : MonoBehaviour
{
    public GameObject enemyPrefab;
    // Range: 0 to 1
    //public float[] levelSpawnRates = new float[3];
    public List<(int x, int y)> spawnPositions = new();
    public abstract bool CheckSpawnPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap);
}
