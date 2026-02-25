using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class FlamethrowerTile : TileBase
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
   // This is shown in the Inspector window when a FlamethrowerTile asset is selected.
   [CustomEditor(typeof(FlamethrowerTile))]
   public class FlamethrowerTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a FlamethrowerTile Asset in the project.
       [MenuItem("Assets/Create/Flamethrower Tile")]
       public static void CreateFlamethrowerTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Flamethrower Tile", "New Flamethrower Tile", "asset", "Save Flamethrower Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<FlamethrowerTile>(), path);
        }
   }
#endif
}

