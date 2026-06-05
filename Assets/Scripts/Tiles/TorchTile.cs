using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class TorchTile : TileBase
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
   // This is shown in the Inspector window when a TorchTile asset is selected.
   [CustomEditor(typeof(TorchTile))]
   public class TorchTileEditor : Editor
   {
       //private TorchTile tile { get { return (target as TorchTile); } }

       // The following is a helper that adds a menu item to create a TorchTile Asset in the project.
       [MenuItem("Assets/Create/Torch Tile")]
       public static void CreateTorchTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Torch Tile", "New Torch Tile", "asset", "Save Torch Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<TorchTile>(), path);
        }
   }
#endif
}

