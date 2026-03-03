using TMPro;
using UnityEngine;

public class HealthCounter : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        text.text = health.CurrentHealth.ToString();
        health.OnHealthChanged += newHealth =>
        {
            text.text = newHealth.ToString();
        };
    }
}
