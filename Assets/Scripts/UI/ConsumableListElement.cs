using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableListElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private Image consumableIcon;
    [SerializeField] private TextMeshProUGUI consumableName;
    [SerializeField] private TextMeshProUGUI consumableCount;
    [SerializeField] private GameObject damageIcon;
    [SerializeField] private TextMeshProUGUI damageAmt;
    public string description;
    
    public void Initialize(ConsumableConfig consumableConfig, int count)
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
}
