using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippedSpellIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int spellIconID = 0;
    
    public delegate void EquippedSpellIconPressed(int spellID);
    public event EquippedSpellIconPressed OnEquippedSpellPressed;
    public delegate void UnequippedSpellIconPressed(int spellID);
    public event UnequippedSpellIconPressed OnUnequippedSpellPressed;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) OnEquippedSpellPressed?.Invoke(spellIconID);
        else if (eventData.button == PointerEventData.InputButton.Right) OnUnequippedSpellPressed?.Invoke(spellIconID);
    }
}
