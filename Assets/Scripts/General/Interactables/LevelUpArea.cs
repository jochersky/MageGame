using UnityEngine;

public class LevelUpArea : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        WishMenuUI menu = FindFirstObjectByType<WishMenuUI>();
        if (!menu)
        {
            Debug.Log("WishMenuUI not found");
            return;
        }
        
        menu.ShowMenu();
    }
}
