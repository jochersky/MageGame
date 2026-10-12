using UnityEngine;

public class WishMenuUI : MonoBehaviour
{
     [SerializeField] private WishButton[] wishButtons;

     private void Start()
     {
          foreach (WishButton wishButton in wishButtons)
          {
               wishButton.OnWishChosen += GrantWish;
          }
     }

     private void GrantWish(Wish wish)
     {
          
     }

     public void ShowMenu()
     {
          UpdateButtons();
          gameObject.SetActive(true);
     }

     private void HideMenu()
     {
          gameObject.SetActive(false);
     }

     private void InitializeButtons()
     {
          Wish healthWish = new Wish()
          {
               wishName = "Vitality",
               statType = StatType.Health,
               increaseAmt = 1
          };
          Wish manaWish = new Wish()
          {
               wishName = "Mana",
               statType = StatType.Mana,
               increaseAmt = 20
          };

          wishButtons[0]?.InitializeWish(healthWish);
          wishButtons[1]?.InitializeWish(manaWish);
     }
     
     private void UpdateButtons()
     {
          foreach (WishButton button in wishButtons)
          {
               button.UpdateWish();
          }
     }
}
