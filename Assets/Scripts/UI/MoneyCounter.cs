using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        InventoryManager.Instance.OnMoneyUpdated += UpdateMoneyCount;
    }
    
    public void UpdateMoneyCount(int amt)
    {
        text.text = amt.ToString();
    }
}
