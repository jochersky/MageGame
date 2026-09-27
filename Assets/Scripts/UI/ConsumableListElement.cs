using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableListElement : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image consumableIcon;
    [SerializeField] private TextMeshProUGUI consumableName;
    [SerializeField] private TextMeshProUGUI consumableCount;
    [SerializeField] private GameObject damageIcon;
    [SerializeField] private TextMeshProUGUI damageAmt;
    [SerializeField] private Button button;
    public string description;
    
    public Button Button => button;

    private int _index;
    
    public void Initialize(ConsumableConfig consumableConfig, int count, int index)
    {
        consumableIcon.sprite = consumableConfig.icon;
        consumableName.text = consumableConfig.itemName;
        consumableCount.text = count.ToString();
        if (consumableConfig.damage > 0)
        {
            damageAmt.text = consumableConfig.damage.ToString();
        }
        else
        {
            damageIcon.SetActive(false);
            damageAmt.gameObject.SetActive(false);
        }
        
        description = consumableConfig.description;
        _index = index;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        InventoryManager.Instance.EquipConsumable(gameObject);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.InventoryUI.UpdateItemDescription(description);
    }
    
    public void HandleOnClick()
    {
        InventoryManager.Instance.EquipConsumable(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameManager.Instance.InventoryUI.UpdateItemDescription(description);
        GameManager.Instance.InventoryUI.UpdateConsumableListDisplay(_index);
    }

    public void OnDeselect(BaseEventData eventData)
    {
    }

    public void UpdateNavigationNeighbors(Selectable upItem, Selectable downItem)
    {
        if (!upItem) return;
        
        var navigation = button.navigation;
        navigation.selectOnUp = upItem;
        button.navigation = navigation;

        if (!downItem) return;
        
        var buttonNavigation = button.navigation;
        buttonNavigation.selectOnDown = downItem;
        button.navigation = buttonNavigation;
    }
}
