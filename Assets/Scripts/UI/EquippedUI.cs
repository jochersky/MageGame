using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        
        // Populate text for labels
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(_equippedConsumableType), _equippedConsumableType);
    }

    private void UpdateConsumableCountUI(int count, ConsumableTypes type)
    {
        if (type != _equippedConsumableType) return;
        
        consumableCountText.text = count.ToString();
    }
    
    public void OnSwitchConsumable(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;
        
        _equippedConsumableType = _equippedConsumableType == ConsumableTypes.Bomb ? ConsumableTypes.BranchTorch : ConsumableTypes.Bomb;
        consumableCountText.text = InventoryManager.Instance.GetConsumableCount(_equippedConsumableType).ToString();
        consumableImage.sprite = _equippedConsumableType == ConsumableTypes.Bomb ? consumableAssets.bombSprite : consumableAssets.branchTorchSprite;
    }
}
