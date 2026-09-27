using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquippedSpellIcon : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private int spellIconID = 0;
    [SerializeField] private GameObject highlight;
    
    public delegate void EquippedSpellIconPressed(int spellID);
    public event EquippedSpellIconPressed OnEquippedSpellPressed;
    public delegate void UnequippedSpellIconPressed(int spellID);
    public event UnequippedSpellIconPressed OnUnequippedSpellPressed;
    
    private void Start()
    {
        highlight.SetActive(false);
    }
    
    // Handles mouse specific controls
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            highlight.SetActive(false);
            OnUnequippedSpellPressed?.Invoke(spellIconID);
        }
    }

    public void HandleOnClick()
    {
        OnEquippedSpellPressed?.Invoke(spellIconID);
    }

    public void DisableHighlight()
    {
        highlight.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        highlight.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        DisableHighlight();
    }
}
