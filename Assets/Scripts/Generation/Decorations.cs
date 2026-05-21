using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Decorations", menuName = "Scriptable Objects/Decorations")]
public class Decorations : ScriptableObject
{
    public List<TileBase> level1DecorFloor = new();
    public List<TileBase> level1DecorCeiling = new();

    public List<TileBase> level2DecorFloor = new();
    public List<TileBase> level2DecorCeiling = new();
    public List<TileBase> level3DecorFloor = new();
    public List<TileBase> level3DecorCeiling = new();
    public List<TileBase> level4DecorFloor = new();
    public List<TileBase> level4DecorCeiling = new();

    

    public TileBase GetRandomDecoration(int level, bool isCeiling)
    {
        System.Random randy = new();
        if (isCeiling)
        {
            return level switch
            {
                1 => level1DecorCeiling[randy.Next(0, level1DecorCeiling.Count)],
                2 => level2DecorCeiling[randy.Next(0, level2DecorCeiling.Count)],
                3 => level3DecorCeiling[randy.Next(0, level3DecorCeiling.Count)],
                4 => level4DecorCeiling[randy.Next(0, level4DecorCeiling.Count)],
                _ => null,
            };
        }
        else
        {
            return level switch
                {
                    1 => level1DecorFloor[randy.Next(0, level1DecorFloor.Count)],
                    2 => level2DecorFloor[randy.Next(0, level2DecorFloor.Count)],
                    3 => level3DecorFloor[randy.Next(0, level3DecorFloor.Count)],
                    4 => level4DecorFloor[randy.Next(0, level4DecorFloor.Count)],
                    _ => null,
                };  
        } 
    }
}
