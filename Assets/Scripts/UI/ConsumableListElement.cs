using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableListElement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image consumableIcon;
    [SerializeField] private TextMeshProUGUI consumableName;
    [SerializeField] private TextMeshProUGUI consumableCount;

    public void Initialize(ConsumableConfig consumableConfig, int count)
    {
        consumableIcon.sprite = consumableConfig.icon;
        consumableName.text = consumableConfig.itemName;
        consumableCount.text = count.ToString();
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        InventoryManager.Instance.EquipConsumable(gameObject);
    }
}
