using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private int money = 0;
    public void AddMoney(int amt)
    {
        money += amt;
        text.text = money.ToString();
    }
}
