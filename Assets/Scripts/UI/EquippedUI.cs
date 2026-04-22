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
    [SerializeField] private ConsumableAssets consumableAssets;
    [SerializeField] private SpellItemAssets spellItemAssets;
    
    private ConsumableTypes _equippedConsumableType = ConsumableTypes.Bomb;
    
    void Start()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableSwitched += UpdateEquippedConsumableUI;
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        InventoryManager.Instance.OnSpell1Equipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Equipped += UpdateEquippedSpell2UI;
        
        // Populate text for labels
        UpdateEquippedConsumableUI(InventoryManager.Instance.EquippedConsumable);
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(_equippedConsumableType), _equippedConsumableType);
        
        spell1Image.enabled = false;
        spell2Image.enabled = false;
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
    
    private void UpdateEquippedSpell1UI(SpellTypes spell)
    {
        spell1Image.enabled = true;
        switch (spell)
        {
            case SpellTypes.FuryOfTheDragon: spell1Image.sprite = spellItemAssets.fireballSprite; break;
            case SpellTypes.GiftOfLight: spell1Image.sprite = spellItemAssets.lightSprite; break;
            case SpellTypes.ReverseFootsteps: spell1Image.sprite = spellItemAssets.reverseFootstepsSprite; break;
        }
    }

    private void UpdateEquippedSpell2UI(SpellTypes spell)
    {
        spell2Image.enabled = true;
        switch (spell)
        {
            case SpellTypes.FuryOfTheDragon: spell2Image.sprite = spellItemAssets.fireballSprite; break;
            case SpellTypes.GiftOfLight: spell2Image.sprite = spellItemAssets.lightSprite; break;
            case SpellTypes.ReverseFootsteps: spell1Image.sprite = spellItemAssets.reverseFootstepsSprite; break;
        }
    } 
}
