using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class ExitTile : TileBase
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
   // This is shown in the Inspector window when a ExitTile asset is selected.
   [CustomEditor(typeof(ExitTile))]
   public class ExitTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a ExitTile Asset in the project.
       [MenuItem("Assets/Create/Exit Tile")]
       public static void CreateExitTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Exit Tile", "New Exit Tile", "asset", "Save Exit Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<ExitTile>(), path);
        }
   }
#endif
}

