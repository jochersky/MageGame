using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject UIElements;
    [SerializeField] private GameObject HUD;

    [Header("Consumables")]
    private ConsumableConfig _consumableConfig1;
    private ConsumableConfig _consumableConfig2;
    [SerializeField] private Image consumable1Image;
    [SerializeField] private Image consumable2Image;
    [SerializeField] private TextMeshProUGUI consumable1Text;
    [SerializeField] private TextMeshProUGUI consumable2Text;
    [SerializeField] private EquippedConsumableIcon equippedConsumable1;
    [SerializeField] private EquippedConsumableIcon equippedConsumable2;
    [SerializeField] private GameObject consumableSelectionMenu;
    [SerializeField] private Transform consumableItemElementSpawnTransform;
    [SerializeField] private GameObject consumableListElementPrefab;
    [SerializeField] private ScrollRect consumableListScrollRect;
    [Header("Spells")]
    [SerializeField] private Image spell1Image;
    [SerializeField] private Image spell2Image;
    [SerializeField] private EquippedSpellIcon equippedSpell1;
    [SerializeField] private EquippedSpellIcon equippedSpell2;
    [SerializeField] private GameObject spellSelectionMenu;
    [SerializeField] private Transform spellItemElementSpawnTransform;
    [SerializeField] private GameObject spellListElementPrefab;
    [SerializeField] private ScrollRect spellListScrollRect;
    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI maxHealthText;
    [SerializeField] private TextMeshProUGUI maxManaText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Image characterPortrait;
    [SerializeField] private Sprite pyroPortrait;
    [SerializeField] private Sprite houndPortrait;
    [SerializeField] private Sprite wardenPortrait;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private TextMeshProUGUI giftDescription;
    
    private EventSystem _eventSystem;
    private GameObject _iconToEquip;
    private ConsumableListElement _firstConsumableSelected;
    private SpellListElement _firstSpellSelected;
    
    private List<ConsumableListElement> _consumableListElements;
    private List<SpellListElement> _spellListElements;

    private void Awake()
    {
        _consumableListElements = new List<ConsumableListElement>();
        _spellListElements = new List<SpellListElement>();
    }
    
    private void Start()
    {
        _eventSystem = FindFirstObjectByType<EventSystem>();
        
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
        
        InventoryManager.Instance.OnMoneyUpdated += (money => moneyText.text = money.ToString());
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
                if (!_consumableConfig1)
                {
                    consumable1Text.text = "0";
                    break;
                }
                consumable1Image.sprite = _consumableConfig1.icon;
                consumable1Text.text = count.ToString();
                break;
            case 2:
                _consumableConfig2 = consumableConfig;
                consumable2Image.color = visible ? Color.white : Color.clear;
                if (!_consumableConfig2)
                {
                    consumable2Text.text = "0";
                    break;
                }
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

    private void AddConsumableToConsumableSelection(ConsumableConfig consumableConfig, int count)
    {
        GameObject inst = Instantiate(consumableListElementPrefab, consumableItemElementSpawnTransform);
        if (inst.TryGetComponent<ConsumableListElement>(out ConsumableListElement listItem))
        {
            _consumableListElements.Add(listItem);
            listItem.Initialize(consumableConfig, count, _consumableListElements.Count-1);
            
            if (_consumableListElements.Count >= 2)
            {
                int newIndex = _consumableListElements.Count - 1;
                for (int i = 1; i < _consumableListElements.Count; i++)
                {
                    int upIndex = i - 1;
                    int downIndex = i != newIndex ? i + 1 : 0;
                    ConsumableListElement item = _consumableListElements[i];
                    item.UpdateNavigationNeighbors(_consumableListElements[upIndex].Button, _consumableListElements[downIndex].Button);
                }
                _firstConsumableSelected.UpdateNavigationNeighbors(_consumableListElements[newIndex].Button, _consumableListElements[1].Button);
            }
            else
            {
                _firstConsumableSelected = listItem;
            }
        }
        InventoryManager.Instance.AddConsumableListItem(inst, consumableConfig);
    }
    
    private void AddSpellToSpellSelection(SpellConfig spellConfig)
    {
        GameObject inst = Instantiate(spellListElementPrefab, spellItemElementSpawnTransform);
        if (inst.TryGetComponent<SpellListElement>(out SpellListElement listItem))
        {
            _spellListElements.Add(listItem);
            listItem.Initialize(spellConfig, _spellListElements.Count-1);
            
            if (_spellListElements.Count >= 2)
            {
                int newIndex = _spellListElements.Count - 1;
                for (int i = 1; i < _spellListElements.Count; i++)
                {
                    int upIndex = i - 1;
                    int downIndex = i != newIndex ? i + 1 : 0;
                    SpellListElement item = _spellListElements[i];
                    item.UpdateNavigationNeighbors(_spellListElements[upIndex].Button, _spellListElements[downIndex].Button);
                }
                _firstSpellSelected.UpdateNavigationNeighbors(_spellListElements[newIndex].Button, _spellListElements[1].Button);
            }
            else
            {
                _firstSpellSelected = listItem;
            }
        }
        InventoryManager.Instance.AddSpellListItem(inst, spellConfig);
    }

    private void ShowConsumableSelectionMenu(int consumableID)
    {
        consumableListScrollRect.verticalNormalizedPosition = 0;
        
        switch (consumableID)
        {
            case 1: 
                equippedConsumable2.DisableHighlight(); 
                _iconToEquip = consumable1Image.gameObject;
                break;
            case 2: 
                equippedConsumable1.DisableHighlight(); 
                _iconToEquip = consumable2Image.gameObject;
                break;
        }
        consumableSelectionMenu.SetActive(true);
        spellSelectionMenu.SetActive(false);
        InventoryManager.Instance.consumableToEquip = consumableID;

        if (_consumableListElements.Count > 0)
        {
            _eventSystem.SetSelectedGameObject(_consumableListElements[0].gameObject);
        }
    }

    public void HideConsumableSelectionMenu()
    {
        consumableSelectionMenu.SetActive(false);
    }
    
    private void ShowSpellSelectionMenu(int spellID)
    {
        switch (spellID)
        {
            case 1: 
                equippedSpell2.DisableHighlight();
                _iconToEquip = spell1Image.gameObject;
                break;
            case 2: 
                equippedSpell1.DisableHighlight();
                _iconToEquip = spell2Image.gameObject;
                break;
        }
        spellSelectionMenu.SetActive(true);
        consumableSelectionMenu.SetActive(false);
        InventoryManager.Instance.spellToEquip = spellID;

        if (_spellListElements.Count > 0)
        {
            _eventSystem.SetSelectedGameObject(_spellListElements[0].gameObject);
        }
    }

    public void HideSpellSelectionMenu()
    {
        spellSelectionMenu.SetActive(false);
    }

    private void UnequipConsumableInSlot(int consumableID)
    {
        InventoryManager.Instance.UnequipConsumable(consumableID);
        UpdateEquippedConsumableUI(consumableID, null, 0, false);
        HideConsumableSelectionMenu();
    }
    
    private void UnequipSpellInSlot(int spellID)
    {
        InventoryManager.Instance.UnequipSpell(spellID);
        if (spellID == 1) UpdateEquippedSpell1UI(null, false);
        else UpdateEquippedSpell2UI(null, false);
        HideSpellSelectionMenu();
    }

    public void UpdateItemDescription(string description)
    {
        itemDescription.text = description;
    }

    public void UpdateStatsScreen(int maxHealth, int maxMana, CharacterType characterType, string gift)
    {
        maxHealthText.text = maxHealth.ToString();
        maxManaText.text = maxMana.ToString();
        moneyText.text = "-1";
        switch (characterType)
        {
            case CharacterType.Base: characterPortrait.sprite = pyroPortrait; characterName.text = "[ character name ]"; break;
            case CharacterType.Pyromancer: characterPortrait.sprite = pyroPortrait; characterName.text = "Pyromancer"; break;
            case CharacterType.Hound: characterPortrait.sprite = houndPortrait; characterName.text = "Hound"; break;
            case CharacterType.Warden: characterPortrait.sprite = wardenPortrait; characterName.text = "Warden"; break;
        }
        giftDescription.text = gift;
    }

    private void ShowHUD()
    {
        HUD.SetActive(true);
    }

    private void HideHUD()
    {
        HUD.SetActive(false);
    }

    public void OnInventoryPressed(bool visible)
    {
        HideConsumableSelectionMenu();
        HideSpellSelectionMenu();
        UIElements.SetActive(visible);
        // HUD.SetActive(!UIElements.activeSelf);
        _eventSystem.SetSelectedGameObject(consumable1Image.gameObject);
    }

    public void UpdateConsumableListDisplay(int index)
    {
        // 1 = top, 0 = bottom
        consumableListScrollRect.verticalNormalizedPosition = 1 - ((float)index / (_consumableListElements.Count - 1));
    }

    public void UpdateSpellListDisplay(int index)
    {
        // 1 = top, 0 = bottom
        spellListScrollRect.verticalNormalizedPosition = 1 - ((float)index / (_spellListElements.Count - 1));
    }
    

    public void OnCancelPressed()
    {
        if (!spellSelectionMenu.activeInHierarchy && !consumableSelectionMenu.activeInHierarchy) return;
        
        HideConsumableSelectionMenu();
        HideSpellSelectionMenu();
        
        _eventSystem.SetSelectedGameObject(_iconToEquip ? _iconToEquip : consumable1Image.gameObject);
        
        UpdateItemDescription("");
    }
}
