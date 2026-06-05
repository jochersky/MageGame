using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConsumableListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI consumableName;

    public void Initialize(ConsumableConfig consumableConfig)
    {
        consumableName.text = consumableConfig.itemName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        InventoryManager.Instance.EquipConsumable(gameObject);
    }
}
