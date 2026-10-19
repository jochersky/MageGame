using UnityEngine;

public class WishGranter : MonoBehaviour, IInteractable
{
    [Header("References")] 
    [SerializeField] private SpriteRenderer outline;
    
    private WishMenuUI _wishMenuUI;
    
    public void Interact()
    {
        if (!_wishMenuUI)
        {
            _wishMenuUI = FindFirstObjectByType<WishMenuUI>();
            
            if (!_wishMenuUI)
            {
                Debug.Log("WishMenuUI not found");
                return;
            }
        }

        if (!_wishMenuUI.MenuOpen) _wishMenuUI.ShowMenu();
        else _wishMenuUI.HideMenu();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) outline.enabled = true;
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) outline.enabled = false;
    }
}
