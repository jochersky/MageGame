using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Explosion : MonoBehaviour
{
    [SerializeField] private Vector2Int explosionSize;
    [SerializeField] private List<Vector2Int> ignoreTiles;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Tilemap>(out Tilemap tilemap))
        {
            for (int x = -explosionSize.x; x <= explosionSize.x; x++)
            {
                for (int y = -explosionSize.y; y <= explosionSize.y; y++)
                {
                    if (ignoreTiles.Contains(new Vector2Int(x, y))) continue;
                    
                    Vector3 worldPos = transform.position;
                    worldPos.x += tilemap.cellSize.x * x;
                    worldPos.y += tilemap.cellSize.y * y;
                    Vector3Int pos = tilemap.WorldToCell(worldPos);
                    if (tilemap.GetTile(pos))
                    {
                        tilemap.SetTile(pos, null);
                    }
                }
            }
        }
    }
}
