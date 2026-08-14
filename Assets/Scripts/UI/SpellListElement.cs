using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellListElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private Image spellIcon;
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private TextMeshProUGUI manaAmt;
    [SerializeField] private GameObject damageIcon;
    [SerializeField] private TextMeshProUGUI damageAmt;
    [SerializeField] private TextMeshProUGUI cooldownAmt;
    public string description;

    public void Initialize(SpellConfig spellConfig)
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
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        InventoryManager.Instance.EquipSpell(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.Instance.InventoryUI.UpdateItemDescription(description);
    }
}
