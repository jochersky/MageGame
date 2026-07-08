using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class DartTrapTile : TileBase
{
    [SerializeField] GameObject TileAssociatedPrefab;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = TileAssociatedPrefab.GetComponent<Sprite>();

        if (TileAssociatedPrefab && tileData.gameObject == null)
        {
            tileData.gameObject = TileAssociatedPrefab;
        } else
        {
            Debug.Log("Error: Tile has no associated prefab");
        }
    }

    #if UNITY_EDITOR
   // This is shown in the Inspector window when a DartTrapTile asset is selected.
   [CustomEditor(typeof(DartTrapTile))]
   public class DartTrapTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a DartTrapTile Asset in the project.
       [MenuItem("Assets/Create/ScriptedTiles/DartTrap Tile")]
       public static void CreateDartTrapTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save DartTrap Tile", "New DartTrap Tile", "asset", "Save DartTrap Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<DartTrapTile>(), path);
        }
   }
#endif
}

