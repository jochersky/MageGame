using TMPro;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        Health health = InventoryManager.Instance.gameObject.GetComponent<Health>();
        text.text = health.CurrentHealth.ToString();
        health.OnHealthChanged += newHealth =>
        {
            text.text = newHealth.ToString();
        };
    }
}
