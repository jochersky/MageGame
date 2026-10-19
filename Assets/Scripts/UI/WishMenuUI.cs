using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WishMenuUI : MonoBehaviour
{
     [Header("References")]
     [SerializeField] private GameObject menu;
     [SerializeField] private TextMeshProUGUI emberCount;
     [SerializeField] private WishButton[] wishButtons;
     [Header("Properties")]
     [SerializeField] private string[] activeActions;

     public bool MenuOpen => menu.activeInHierarchy;
     
     private EventSystem _eventSystem;
     
     private void Start()
     {
          _eventSystem = FindFirstObjectByType<EventSystem>();
          
          // InputManager.Instance.OnPlayerPressedInteract += CheckToCloseWithInteract;
          InputManager.Instance.OnPlayerPressedCancel += HideMenu;
          
          InitializeButtons();
          
          foreach (WishButton wishButton in wishButtons)
          {
               wishButton.OnWishChosen += GrantWish;
          }
     }

     private void GrantWish(Wish wish)
     {
          GameManager.Instance.Player.LevelUp(wish);
          emberCount.text = $"Embers: \n{GameManager.Instance.Player.Embers}";
          UpdateButtons();
     }

     public void ShowMenu()
     {
          UpdateButtons();
          emberCount.text = $"Embers: \n{GameManager.Instance.Player.Embers}";
          
          menu.SetActive(true);
          _eventSystem.SetSelectedGameObject(wishButtons[0].gameObject);
          InputManager.Instance.ToggleInputsBesides(activeActions, true);
     }

     public void HideMenu()
     {
          if (!menu.activeInHierarchy) return;
          
          menu.SetActive(false);
          InputManager.Instance.ToggleInputsBesides(activeActions, false);
     }

     private void InitializeButtons()
     {
          // As of now there are only a set amount of wishes to make,
          // TODO: in the future this could be randomized to random wishes
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
               button.ToggleButton(GameManager.Instance.Player.CanGetWish);
          }
     }

     private void CheckToCloseWithInteract()
     {
          // Toggle menu
          // menu.SetActive(!menu.activeInHierarchy);
          
          // bool menuVisible = menu.activeInHierarchy;
          // if (!menuVisible) InputManager.Instance.ToggleInputsBesides(activeActions, menuVisible);
          // if (menuVisible) ShowMenu();
     }
}
