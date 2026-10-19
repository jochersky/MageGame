using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WishButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wishName;
    [SerializeField] private Button button;
    
    private Wish _currentWish;
    
    public delegate void WishChosen(Wish wish);
    public event WishChosen OnWishChosen;

    public void InitializeWish(Wish wish)
    {
        _currentWish = wish;
        if (wishName.text == "") wishName.text = _currentWish.statType.ToString();
    }

    public void UpdateWish()
    {
        // Called whenever wish menu is shown to provide most up-to-date stat 
    }

    public void ToggleButton(bool interactable)
    {
        button.interactable = interactable;
    }

    public void OnWishPressed()
    {
        OnWishChosen?.Invoke(_currentWish);
    }
}
