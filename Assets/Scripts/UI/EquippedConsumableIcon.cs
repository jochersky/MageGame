using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedConsumableIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int consumableIconID = 0;
    
    public delegate void EquippedConsumableIconPressed(int spellID);
    public event EquippedConsumableIconPressed OnEquippedConsumablePressed;
    public delegate void UnequippedConsumableIconPressed(int spellID);
    public event UnequippedConsumableIconPressed OnUnequippedConsumablePressed;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnEquippedConsumablePressed?.Invoke(consumableIconID);
        else if (eventData.button == PointerEventData.InputButton.Right) OnUnequippedConsumablePressed?.Invoke(consumableIconID);
    }
}
