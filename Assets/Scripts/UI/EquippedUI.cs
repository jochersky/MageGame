using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquippedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI consumableCountText;
    [SerializeField] private Image consumableImage;
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell2Image;

    private ConsumableConfig _consumableConfig1;
    private ConsumableConfig _consumableConfig2;
    private ConsumableConfig _equippedConsumable;
    
    void Start()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableSwitched += UpdateEquippedConsumableUI;
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        InventoryManager.Instance.OnSpell1Equipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Equipped += UpdateEquippedSpell2UI;
        InventoryManager.Instance.OnSpell1Unequipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Unequipped += UpdateEquippedSpell2UI;
        
        spell1Image.enabled = false;
        spell2Image.enabled = false;
    }

    private void UpdateEquippedConsumableUI(ConsumableConfig config, int amount)
    {
        Debug.Log("updating " + config.itemName);
        _equippedConsumable = config;
        UpdateConsumableCountUI(config, amount);
        consumableImage.sprite = config.icon;
    }
    
    private void UpdateConsumableCountUI(ConsumableConfig config, int count)
    {
        // don't update the text if the config isn't the same as the equipped config
        if (config != _equippedConsumable) return;
        consumableCountText.text = count.ToString();
    }
    
    private void UpdateEquippedSpell1UI(Sprite spellSprite, bool visible)
    {
        spell1Image.enabled = visible;
        spell1Image.sprite = spellSprite;
    }

    private void UpdateEquippedSpell2UI(Sprite spellSprite, bool visible)
    {
        spell2Image.enabled = visible;
        spell2Image.sprite = spellSprite;
    } 
}
