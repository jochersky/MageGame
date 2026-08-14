using UnityEngine;
using System.Collections;

public class EventBus : MonoBehaviour
{
    // Singleton instance
    public static EventBus Instance { get; private set; }

    public delegate void TileMapChanged();
    public event TileMapChanged OnTileMapChanged;

    public delegate void ChestOpened(bool chestInteractable);
    public event ChestOpened OnChestOpened;
    
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
        // ok for this coroutine to be interrupted and called
        StartCoroutine(UpdateTileMapAtEndOfFrame());
    }

    IEnumerator UpdateTileMapAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        OnTileMapChanged?.Invoke();
    }
    
    public void HandleChestOpened(bool chestInteractable) 
    {
        OnChestOpened?.Invoke(chestInteractable);    
    }
}
