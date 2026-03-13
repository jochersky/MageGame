using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI spellName;

    public void Initialize(Spell spell)
    {
        spellName.text = spell.spellType.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        InventoryManager.Instance.EquipSpell(this.gameObject);
        Debug.Log("Pressing to equip " + spellName.text);
    }
}
