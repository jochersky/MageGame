using UnityEngine;
using UnityEngine.EventSystems;

public class EquippedConsumableIcon : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int consumableIconID = 0;
    [SerializeField] private GameObject highlight;
    
    public delegate void EquippedConsumableIconPressed(int spellID);
    public event EquippedConsumableIconPressed OnEquippedConsumablePressed;
    public delegate void UnequippedConsumableIconPressed(int spellID);
    public event UnequippedConsumableIconPressed OnUnequippedConsumablePressed;

    private void Start()
    {
        highlight.SetActive(false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            highlight.SetActive(true);
            OnEquippedConsumablePressed?.Invoke(consumableIconID);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            highlight.SetActive(false);
            OnUnequippedConsumablePressed?.Invoke(consumableIconID);
        }
    }
    
    public void DisableHighlight()
    {
        highlight.SetActive(false);
    }
}
