using NavMeshPlus.Components;
using UnityEngine;

public class NavigationSurfaceManager : MonoBehaviour
{
    [SerializeField] private NavMeshSurface spiderNavMeshSurface;

    private void Start()
    {
        SyncNavMesh();
        
        EventBus.Instance.OnTileMapChanged += SyncNavMesh;
    }
    
    private void SyncNavMesh()
    {
        spiderNavMeshSurface.UpdateNavMesh(spiderNavMeshSurface.navMeshData);
    }
}
