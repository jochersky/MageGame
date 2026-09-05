using UnityEngine;

public class HUDUI : MonoBehaviour
{
    [SerializeField] private GameObject addToInventoryText;

    private void Start()
    {
        EventBus.Instance.OnChestOpened += chestInteractable =>
        {
            if (addToInventoryText) addToInventoryText?.SetActive(chestInteractable);
        };
    }
}
