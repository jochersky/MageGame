using UnityEngine;

public class HUDUI : MonoBehaviour
{
    [SerializeField] private GameObject addToInventoryText;

    private void Start()
    {
        EventBus.Instance.OnChestOpened += chestInteractable => { addToInventoryText.SetActive(chestInteractable); };
    }
}
