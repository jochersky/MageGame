using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bombCountText;
    [SerializeField] private TextMeshProUGUI branchTorchCountText;
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell2Image;
    [SerializeField] private GameObject spellSelectionMenu;
    [SerializeField] private GameObject spellListItemPrefab;
    [SerializeField] private ConsumableAssets consumableAssets;
    [SerializeField] private SpellItemAssets spellItemAssets;
    
    void Start()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        InventoryManager.Instance.OnSpell1Equipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Equipped += UpdateEquippedSpell2UI;
        InventoryManager.Instance.OnSpellAdded += AddSpellToSpellSelection;
        
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(ConsumableTypes.Bomb), ConsumableTypes.Bomb);
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(ConsumableTypes.BranchTorch), ConsumableTypes.BranchTorch);
        
        spell1Image.enabled = false;
        spell2Image.enabled = false;
    }
    
    private void UpdateConsumableCountUI(int count, ConsumableTypes type)
    {
        switch (type)
        {
            case ConsumableTypes.Bomb: bombCountText.text = count.ToString(); break;
            case ConsumableTypes.BranchTorch: branchTorchCountText.text = count.ToString(); break;
        }
    }

    private void UpdateEquippedSpell1UI(SpellTypes spell)
    {
        spell1Image.enabled = true;
        switch (spell)
        {
            case SpellTypes.FuryOfTheDragon: spell1Image.sprite = spellItemAssets.fireballSprite; break;
            case SpellTypes.GiftOfLight: spell1Image.sprite = spellItemAssets.lightSprite; break;
        }
    }

    private void UpdateEquippedSpell2UI(SpellTypes spell)
    {
        spell2Image.enabled = true;
        switch (spell)
        {
            case SpellTypes.FuryOfTheDragon: spell2Image.sprite = spellItemAssets.fireballSprite; break;
            case SpellTypes.GiftOfLight: spell2Image.sprite = spellItemAssets.lightSprite; break;
        }
    }

    // TODO: connect this to InventoryManager event when spell is added
    private void AddSpellToSpellSelection(Spell spell)
    {
        GameObject inst = Instantiate(spellListItemPrefab, spellSelectionMenu.transform);
        if (inst.TryGetComponent<SpellListItem>(out SpellListItem listItem))
        {
            listItem.Initialize(spell);
        }
    }
}
