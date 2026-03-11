using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class BarrelTile : TileBase
{
    [SerializeField] Sprite TileSprite;
    [SerializeField] GameObject TileAssociatedPrefab;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = TileSprite;

        if (TileAssociatedPrefab && tileData.gameObject == null)
        {
            tileData.gameObject = TileAssociatedPrefab;
        } else
        {
            Debug.Log("Error: Tile has no associated prefab");
        }
    }

    #if UNITY_EDITOR
   // This is shown in the Inspector window when a BarrelTile asset is selected.
   [CustomEditor(typeof(BarrelTile))]
   public class BarrelTileEditor : Editor
   {
       // The following is a helper that adds a menu item to create a BarrelTile Asset in the project.
       [MenuItem("Assets/Create/Barrel Tile")]
       public static void CreateBarrelTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Barrel Tile", "New Barrel Tile", "asset", "Save Barrel Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<BarrelTile>(), path);
        }
   }
#endif
}

