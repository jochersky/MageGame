using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIElements;
    [SerializeField] private GameObject HUD;

    private ConsumableConfig _consumableConfig1;
    private ConsumableConfig _consumableConfig2;
    [SerializeField] private Image consumable1Image;
    [SerializeField] private Image consumable2Image;
    [SerializeField] private TextMeshProUGUI consumable1Text;
    [SerializeField] private TextMeshProUGUI consumable2Text;
    [SerializeField] private EquippedConsumableIcon equippedConsumable1;
    [SerializeField] private EquippedConsumableIcon equippedConsumable2;
    [SerializeField] private GameObject consumableSelectionMenu;
    [SerializeField] private GameObject consumableListItemPrefab;
    
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell2Image;
    [SerializeField] private EquippedSpellIcon equippedSpell1;
    [SerializeField] private EquippedSpellIcon equippedSpell2;
    [SerializeField] private GameObject spellSelectionMenu;
    [SerializeField] private GameObject spellListItemPrefab;
    
    void Start()
    {
        HideConsumableSelectionMenu();
        HideSpellSelectionMenu();
        UIElements.SetActive(false);
    }

    private void OnEnable()
    {
        // Subscribe to events
        InventoryManager.Instance.OnConsumableCountUpdated += UpdateConsumableCountUI;
        InventoryManager.Instance.OnConsumable1Equipped += UpdateEquippedConsumableUI;
        InventoryManager.Instance.OnConsumable2Equipped += UpdateEquippedConsumableUI;
        equippedConsumable1.OnEquippedConsumablePressed += ShowConsumableSelectionMenu;
        equippedConsumable2.OnEquippedConsumablePressed += ShowConsumableSelectionMenu;
        equippedConsumable1.OnUnequippedConsumablePressed += UnequipConsumableInSlot;
        equippedConsumable2.OnUnequippedConsumablePressed += UnequipConsumableInSlot;
        InventoryManager.Instance.OnConsumableAdded += AddConsumableToConsumableSelection;
        
        InventoryManager.Instance.OnSpell1Equipped += UpdateEquippedSpell1UI;
        InventoryManager.Instance.OnSpell2Equipped += UpdateEquippedSpell2UI;
        equippedSpell1.OnEquippedSpellPressed += ShowSpellSelectionMenu;
        equippedSpell2.OnEquippedSpellPressed += ShowSpellSelectionMenu;
        equippedSpell1.OnUnequippedSpellPressed += UnequipSpellInSlot;
        equippedSpell2.OnUnequippedSpellPressed += UnequipSpellInSlot;
        InventoryManager.Instance.OnSpellAdded += AddSpellToSpellSelection;
    }
    
    private void UpdateConsumableCountUI(ConsumableConfig consumableConfig, int count)
    {
        if (consumableConfig == _consumableConfig1) consumable1Text.text = count.ToString();
        else if (consumableConfig == _consumableConfig2) consumable2Text.text = count.ToString();
    }

    private void UpdateEquippedConsumableUI(int equipSlot, ConsumableConfig consumableConfig, int count, bool visible)
    {
        switch (equipSlot)
        {
            case 1:
                _consumableConfig1 = consumableConfig;
                consumable1Image.color = visible ? Color.white : Color.clear;
                consumable1Image.sprite = _consumableConfig1.icon;
                consumable1Text.text = count.ToString();
                break;
            case 2:
                _consumableConfig2 = consumableConfig;
                consumable2Image.color = visible ? Color.white : Color.clear;
                consumable2Image.sprite = _consumableConfig2.icon;
                consumable2Text.text = count.ToString();
                break;
        }
    }

    private void UpdateEquippedSpell1UI(Sprite spellSprite, bool visible)
    {
        spell1Image.color = visible ? Color.white : Color.clear;
        spell1Image.sprite = spellSprite;
    }

    private void UpdateEquippedSpell2UI(Sprite spellSprite, bool visible)
    {
        spell2Image.color = visible ? Color.white : Color.clear;
        spell2Image.sprite = spellSprite;
    }

    private void AddConsumableToConsumableSelection(ConsumableConfig consumableConfig)
    {
        GameObject inst = Instantiate(consumableListItemPrefab, consumableSelectionMenu.transform);
        if (inst.TryGetComponent<ConsumableListItem>(out ConsumableListItem listItem))
        {
            listItem.Initialize(consumableConfig);
        }
        InventoryManager.Instance.AddConsumableListItem(inst, consumableConfig);
    }
    
    private void AddSpellToSpellSelection(SpellConfig spellConfig)
    {
        GameObject inst = Instantiate(spellListItemPrefab, spellSelectionMenu.transform);
        if (inst.TryGetComponent<SpellListItem>(out SpellListItem listItem))
        {
            listItem.Initialize(spellConfig);
        }
        InventoryManager.Instance.AddSpellListItem(inst, spellConfig);
    }

    private void ShowConsumableSelectionMenu(int consumableID)
    {
        consumableSelectionMenu.SetActive(true);
        InventoryManager.Instance.consumableToEquip = consumableID;
    }

    public void HideConsumableSelectionMenu()
    {
        consumableSelectionMenu.SetActive(false);
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

    private void UnequipConsumableInSlot(int consumableID)
    {
        // InventoryManager.Instance.Une
        UpdateEquippedConsumableUI(consumableID, null, 0, false);
    }
    
    private void UnequipSpellInSlot(int spellID)
    {
        InventoryManager.Instance.UnequipSpell(spellID);
        if (spellID == 1) UpdateEquippedSpell1UI(null, false);
        else UpdateEquippedSpell2UI(null, false);
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
        
        HideConsumableSelectionMenu();
        HideSpellSelectionMenu();
        UIElements.SetActive(!UIElements.activeSelf);
        HUD.SetActive(!UIElements.activeSelf);
    }
}
