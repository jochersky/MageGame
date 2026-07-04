using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public RuleTile ruleTile;
    public List<EnemyInfo> enemies;
    public List<int> enemySpawnCounts;
    public int numDecorations = 100;
    public List<TileBase> decorFloor = new();
    public List<TileBase> decorCeiling = new();
    // public GameObject smashable;
    public TileBase falseFloor;
    public TileBase spikes;
    public TileBase exitDoor;
    public TileBase GetRandomDecoration(bool isCeiling)
    {
        System.Random randy = new();
        if (isCeiling)
        {
            return decorCeiling[randy.Next(0, decorCeiling.Count)];
        }
        else
        {
            return decorFloor[randy.Next(0, decorFloor.Count)];
        } 
    }
}
