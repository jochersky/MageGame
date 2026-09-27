using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellListElement : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image spellIcon;
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private TextMeshProUGUI manaAmt;
    [SerializeField] private GameObject damageIcon;
    [SerializeField] private TextMeshProUGUI damageAmt;
    [SerializeField] private TextMeshProUGUI cooldownAmt;
    [SerializeField] private Button button;
    public string description;
    
    public Button Button => button;

    private int _index;
    
    public void Initialize(SpellConfig spellConfig, int index)
    {
        spellIcon.sprite = spellConfig.icon;
        spellName.text = spellConfig.itemName;
        manaAmt.text = spellConfig.manaCost.ToString();
        if (spellConfig.strategy != null && spellConfig.strategy.damage > 0)
        {
            damageAmt.text = spellConfig.strategy.damage.ToString();
        }
        else
        {
            damageIcon.SetActive(false);
            damageAmt.gameObject.SetActive(false);
        }
        cooldownAmt.text = spellConfig.cooldown.ToString();
        
        description = spellConfig.description;
        _index = index;
    }
    
    // For mouse
    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     if (eventData.button != PointerEventData.InputButton.Left) return;
    //     
    //     HandleOnClick();
    // }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.InventoryUI.UpdateItemDescription(description);
    }

    public void HandleOnClick()
    {
        InventoryManager.Instance.EquipSpell(gameObject);
    }

    public void OnSelect(BaseEventData eventData)
    {
        GameManager.Instance.InventoryUI.UpdateItemDescription(description);
        GameManager.Instance.InventoryUI.UpdateSpellListDisplay(_index);
    }

    public void OnDeselect(BaseEventData eventData)
    {
    }

    public void UpdateNavigationNeighbors(Selectable upItem, Selectable downItem)
    {
        if (!upItem) return;
        
        var navigation = button.navigation;
        navigation.selectOnUp = upItem;
        button.navigation = navigation;

        if (!downItem) return;
        
        var buttonNavigation = button.navigation;
        buttonNavigation.selectOnDown = downItem;
        button.navigation = buttonNavigation;
    }
}
