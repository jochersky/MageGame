using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI spellName;
    [SerializeField] private GameObject equippedIcon;

    public void Initialize(Spell spell)
    {
        spellName.text = spell.spellType.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (InventoryManager.Instance.EquipSpell(gameObject))
        {
            equippedIcon.SetActive(true);
        }
    }
}
