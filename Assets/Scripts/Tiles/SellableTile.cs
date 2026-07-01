using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class SellableTile : TileBase
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
   // This is shown in the Inspector window when a SellableTile asset is selected.
   [CustomEditor(typeof(SellableTile))]
   public class SellableTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a SellableTile Asset in the project.
       [MenuItem("Assets/Create/ScriptedTiles/Sellable Tile")]
       public static void CreateSellableTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Sellable Tile", "New Sellable Tile", "asset", "Save Sellable Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<SellableTile>(), path);
        }
   }
#endif
}

