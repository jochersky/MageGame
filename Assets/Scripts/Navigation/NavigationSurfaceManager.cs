using NavMeshPlus.Components;
using UnityEngine;

public class NavigationSurfaceManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface spiderNavMeshSurface;
    
    private void Start()
    {
        spiderNavMeshSurface.BuildNavMesh();
        SyncNavMesh();
        
        EventBus.Instance.OnTileMapChanged += SyncNavMesh;
    }
    
    private void SyncNavMesh()
    {
        // check added to get rid of missing reference error when changing scenes
        if (!spiderNavMeshSurface) return;

        spiderNavMeshSurface.UpdateNavMesh(spiderNavMeshSurface.navMeshData);
    }
}
