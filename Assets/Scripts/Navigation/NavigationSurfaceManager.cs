using NavMeshPlus.Components;
using UnityEngine;

public class NavigationSurfaceManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface spiderNavMeshSurface;
    [SerializeField] private NavMeshSurface jungleSpiderNavMeshSurface;
    
    private void Start()
    {
        if (spiderNavMeshSurface) spiderNavMeshSurface.BuildNavMesh();
        if (jungleSpiderNavMeshSurface) jungleSpiderNavMeshSurface.BuildNavMesh();
        SyncNavMesh();
        
        EventBus.Instance.OnTileMapChanged += SyncNavMesh;
    }
    
    private void SyncNavMesh()
    {
        // check added to get rid of missing reference error when changing scenes
        if (spiderNavMeshSurface) spiderNavMeshSurface.UpdateNavMesh(spiderNavMeshSurface.navMeshData);
        if (jungleSpiderNavMeshSurface) jungleSpiderNavMeshSurface.UpdateNavMesh(jungleSpiderNavMeshSurface.navMeshData);
        
    }
}
