using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class PortalTile : TileBase
{
    [SerializeField] GameObject TileAssociatedPrefab;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //tileData.sprite = TileAssociatedPrefab.GetComponent<SpriteRenderer>().sprite;

        if (TileAssociatedPrefab && tileData.gameObject == null)
        {
            tileData.gameObject = TileAssociatedPrefab;
        } else
        {
            Debug.Log("Error: Tile has no associated prefab");
        }
    }

    #if UNITY_EDITOR
   // This is shown in the Inspector window when a PortalTile asset is selected.
   [CustomEditor(typeof(PortalTile))]
   public class PortalTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a PortalTile Asset in the project.
       [MenuItem("Assets/Create/ScriptedTiles/Portal Tile")]
       public static void CreatePortalTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Portal Tile", "New Portal Tile", "asset", "Save Portal Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<PortalTile>(), path);
        }
   }
#endif
}

