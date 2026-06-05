using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI spellName;

    public void Initialize(SpellConfig spellConfig)
    {
        spellName.text = spellConfig.itemName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        InventoryManager.Instance.EquipSpell(gameObject);
    }
}
