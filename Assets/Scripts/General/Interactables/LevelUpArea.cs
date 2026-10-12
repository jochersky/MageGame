using UnityEngine;

public class LevelUpArea : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Player player = FindFirstObjectByType<Player>();
        if (!player)
        {
            Debug.Log("Player not found");
            return;
        }
        
        // TODO: change to a UI pop up which allows player to make selection for upgrade.
        // LevelUp will be called by the UI 
        Debug.Log(player.LevelUp());
    }
}
