using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellListElement : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image spellIcon;
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private TextMeshProUGUI spellStats;

    public void Initialize(SpellConfig spellConfig)
    {
        spellIcon.sprite = spellConfig.icon;
        spellName.text = spellConfig.itemName;
        spellStats.text = "Mana: " + spellConfig.manaCost;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        InventoryManager.Instance.EquipSpell(gameObject);
    }
}
