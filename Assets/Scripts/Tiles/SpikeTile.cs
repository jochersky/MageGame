using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class SpikeTile : TileBase
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
   // This is shown in the Inspector window when a SpikeTile asset is selected.
   [CustomEditor(typeof(SpikeTile))]
   public class SpikeTileEditor : Editor
   {
       //private SpikeTile tile { get { return (target as SpikeTile); } }

       // The following is a helper that adds a menu item to create a SpikeTile Asset in the project.
       [MenuItem("Assets/Create/Spike Tile")]
       public static void CreateSpikeTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save Spike Tile", "New Spike Tile", "asset", "Save Spike Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<SpikeTile>(), path);
        }
   }
#endif
}

