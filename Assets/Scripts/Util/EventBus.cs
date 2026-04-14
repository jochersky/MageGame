using UnityEngine;
using System.Collections;

public class EventBus : MonoBehaviour
{
    // Singleton instance
    public static EventBus Instance { get; private set; }

    public delegate void TileMapChanged();
    public event TileMapChanged OnTileMapChanged;
    
    
    private void Awake()
    {
        // Ensure only one instance of the inventory exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void HandleTileMapChanged()
    {
        StartCoroutine(UpdateTileMapAtEndOfFrame());
    }

    IEnumerator UpdateTileMapAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        OnTileMapChanged?.Invoke();
    }
}
