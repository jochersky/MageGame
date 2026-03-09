using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquippedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consumableCountText;
    [SerializeField] private Image consumableImage;
    [SerializeField] private ConsumableAssets consumableAssets;
    
    private ConsumableTypes _equippedConsumableType = ConsumableTypes.Bomb;
    
    void Start()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableSwitched += UpdateEquippedConsumableUI;
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        
        // Populate text for labels
        UpdateEquippedConsumableUI(InventoryManager.Instance.EquippedConsumable);
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(_equippedConsumableType), _equippedConsumableType);
    }

    private void UpdateEquippedConsumableUI(ConsumableTypes consumableType)
    {
        consumableCountText.text = InventoryManager.Instance.GetConsumableCount(consumableType).ToString();
        consumableImage.sprite = consumableType == ConsumableTypes.Bomb ? consumableAssets.bombSprite : consumableAssets.branchTorchSprite;
    }
    
    private void UpdateConsumableCountUI(int count, ConsumableTypes type)
    {
        if (type != _equippedConsumableType) return;
        
        consumableCountText.text = count.ToString();
    }
}
