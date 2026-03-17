using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIElements;
    [SerializeField] private GameObject HUD;
    [SerializeField] private TextMeshProUGUI bombCountText;
    [SerializeField] private TextMeshProUGUI branchTorchCountText;
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell2Image;
    [SerializeField] private Image passiveSpell1Image;
    [SerializeField] private Image passiveSpell2Image;
    [SerializeField] private EquippedSpellIcon equippedSpell1;
    [SerializeField] private EquippedSpellIcon equippedSpell2;
    [SerializeField] private EquippedSpellIcon equippedPassiveSpell1;
    [SerializeField] private EquippedSpellIcon equippedPassiveSpell2;
    [SerializeField] private GameObject spellSelectionMenu;
    [SerializeField] private GameObject passiveSpellSelectionMenu;
    [SerializeField] private GameObject spellListItemPrefab;
    [SerializeField] private GameObject passiveSpellListItemPrefab;
    [SerializeField] private ConsumableAssets consumableAssets;
    [SerializeField] private SpellItemAssets spellItemAssets;
    
    void Start()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        InventoryManager.Instance.OnSpell1Equipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Equipped += UpdateEquippedSpell2UI;
        InventoryManager.Instance.OnPassiveSpell1Equipped += UpdateEquippedPassiveSpell1UI;
        InventoryManager.Instance.OnPassiveSpell2Equipped += UpdateEquippedPassiveSpell2UI;
        equippedSpell1.OnEquippedSpellPressed += ShowSpellSelectionMenu;
        equippedSpell2.OnEquippedSpellPressed += ShowSpellSelectionMenu;
        equippedPassiveSpell1.OnEquippedSpellPressed += ShowPassiveSpellSelectionMenu;
        equippedPassiveSpell2.OnEquippedSpellPressed += ShowPassiveSpellSelectionMenu;
        InventoryManager.Instance.OnSpellAdded += AddActiveSpellToSpellSelection;
        InventoryManager.Instance.OnPassiveSpellAdded += AddSpellToPassiveSpellSelection;
        
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(ConsumableTypes.Bomb), ConsumableTypes.Bomb);
        UpdateConsumableCountUI(InventoryManager.Instance.GetConsumableCount(ConsumableTypes.BranchTorch), ConsumableTypes.BranchTorch);
        
        spell1Image.enabled = false;
        spell2Image.enabled = false;
        HideSpellSelectionMenu();
        HidePassiveSpellSelectionMenu();
        UIElements.SetActive(false);
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

    private void UpdateEquippedPassiveSpell1UI(SpellTypes spell)
    {
        passiveSpell1Image.enabled = true;
        switch (spell)
        {
            case SpellTypes.WindLordsBlessing: passiveSpell1Image.sprite = spellItemAssets.bounceSprite; break;
        }
    }
    
    private void UpdateEquippedPassiveSpell2UI(SpellTypes spell)
    {
        passiveSpell2Image.enabled = true;
        switch (spell)
        {
            case SpellTypes.WindLordsBlessing: passiveSpell2Image.sprite = spellItemAssets.bounceSprite; break;
        }
    }

    private void AddActiveSpellToSpellSelection(ActiveSpell spell)
    {
        GameObject inst = Instantiate(spellListItemPrefab, spellSelectionMenu.transform);
        if (inst.TryGetComponent<SpellListItem>(out SpellListItem listItem))
        {
            listItem.Initialize(spell);
        }
        InventoryManager.Instance.AddSpellListItem(inst, spell);
    }

    private void AddSpellToPassiveSpellSelection(PassiveSpell spell)
    {
        GameObject inst = Instantiate(passiveSpellListItemPrefab, passiveSpellSelectionMenu.transform);
        if (inst.TryGetComponent<PassiveSpellListItem>(out PassiveSpellListItem listItem))
        {
            listItem.Initialize(spell);
        }
        InventoryManager.Instance.AddPassiveSpellListItem(inst, spell);
    }

    private void ShowSpellSelectionMenu(int spellID)
    {
        spellSelectionMenu.SetActive(true);
        InventoryManager.Instance.spellToEquip = spellID;
    }

    public void HideSpellSelectionMenu()
    {
        spellSelectionMenu.SetActive(false);
    }

    private void ShowPassiveSpellSelectionMenu(int spellID)
    {
        passiveSpellSelectionMenu.SetActive(true);
        InventoryManager.Instance.passiveSpellToEquip = spellID;
    }

    public void HidePassiveSpellSelectionMenu()
    {
        passiveSpellSelectionMenu.SetActive(false);
    }

    private void ShowHUD()
    {
        HUD.SetActive(true);
    }

    private void HideHUD()
    {
        HUD.SetActive(false);
    }

    public void OnInventoryPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;
        
        HideSpellSelectionMenu();
        HidePassiveSpellSelectionMenu();
        UIElements.SetActive(!UIElements.activeSelf);
        HUD.SetActive(!UIElements.activeSelf);
    }
}
