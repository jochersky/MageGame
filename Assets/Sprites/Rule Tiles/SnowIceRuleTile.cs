using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu]
public class SnowIceRuleTile : RuleTile<SnowIceRuleTile.Neighbor> {
    public TileBase[] connectableTiles;

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int selfOrConnectable = 1;
        public const int noTile = 2;
        public const int self = 3;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.self: return this == tile;
            case Neighbor.selfOrConnectable: return IsConnectable(tile);
            case Neighbor.noTile: return tile == null;
        }
        return base.RuleMatch(neighbor, tile);
    }

    private bool IsConnectable(TileBase other)
    {
        if (other == null)
        {
            return false;
        }
        if (connectableTiles.Contains<TileBase>(other) || this == other)
        {
            return true;
        } else
        {
            return false;
        }
    }
}

//     #if UNITY_EDITOR
//    // This is shown in the Inspector window when a SnowIceRuleTile asset is selected.
//    [CustomEditor(typeof(SnowIceRuleTile))]
//    public class SnowIceRuleTileEditor : RuleTileEditor
//    {
//     public override void OnInspectorGUI()
//     {

//         base.OnInspectorGUI();
//     }
//    }
// #endif