using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class LandmineTile : TileBase
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
   // This is shown in the Inspector window when a LandmineTile asset is selected.
   [CustomEditor(typeof(LandmineTile))]
   public class LandmineTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a LandmineTile Asset in the project.
       [MenuItem("Assets/Create/ScriptedTiles/Landmine Tile")]
       public static void CreateLandmineTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Landmine Tile", "New Landmine Tile", "asset", "Save Landmine Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<LandmineTile>(), path);
        }
   }
#endif
}

