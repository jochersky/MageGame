using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class FalseFloorTile : TileBase
{
    //[SerializeField] Sprite TileSprite;
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
   // This is shown in the Inspector window when a FalseFloorTile asset is selected.
   [CustomEditor(typeof(FalseFloorTile))]
   public class FalseFloorTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a FalseFloorTile Asset in the project.
       [MenuItem("Assets/Create/FalseFloor Tile")]
       public static void CreateFalseFloorTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save FalseFloor Tile", "New FalseFloor Tile", "asset", "Save FalseFloor Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<FalseFloorTile>(), path);
        }
   }
#endif
}

