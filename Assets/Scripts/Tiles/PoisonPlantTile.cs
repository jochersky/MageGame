using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class PoisonPlantTile : TileBase
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
   // This is shown in the Inspector window when a PoisonPlantTile asset is selected.
   [CustomEditor(typeof(PoisonPlantTile))]
   public class PoisonPlantTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a PoisonPlantTile Asset in the project.
       [MenuItem("Assets/Create/PoisonPlant Tile")]
       public static void CreatePoisonPlantTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save PoisonPlant Tile", "New PoisonPlant Tile", "asset", "Save PoisonPlant Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<PoisonPlantTile>(), path);
        }
   }
#endif
}

