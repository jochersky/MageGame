using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class SpikeFruitTile : TileBase
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
   // This is shown in the Inspector window when a SpikeFruitTile asset is selected.
   [CustomEditor(typeof(SpikeFruitTile))]
   public class SpikeFruitTileEditor : Editor
   {

       // The following is a helper that adds a menu item to create a SpikeFruitTile Asset in the project.
       [MenuItem("Assets/Create/ScriptedTiles/SpikeFruit Tile")]
       public static void CreateSpikeFruitTile()
       {
           string path = EditorUtility.SaveFilePanelInProject("Save SpikeFruit Tile", "New SpikeFruit Tile", "asset", "Save SpikeFruit Tile", "Assets");
           if (path == "")
               return;                           
           AssetDatabase.CreateAsset(CreateInstance<SpikeFruitTile>(), path);
        }
   }
#endif
}

