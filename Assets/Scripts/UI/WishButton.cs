using TMPro;
using UnityEngine;

public class WishButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wishName;
    
    private Wish _currentWish;
    
    public delegate void WishChosen(Wish wish);
    public event WishChosen OnWishChosen;

    public void InitializeWish(Wish wish)
    {
        _currentWish = wish;
        wishName.text = _currentWish.statType.ToString();
    }

    public void UpdateWish()
    {
        // Called whenever wish menu is shown to provide most up-to-date stat 
    }

    public void OnWishPressed()
    {
        OnWishChosen?.Invoke(_currentWish);
    }
}
