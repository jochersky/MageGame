using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EmberCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        GameManager.Instance.Player.OnEmbersUpdated += UpdateEmberCount;
    }
    
    public void UpdateEmberCount(int amt)
    {
        text.text = "*~ " + amt.ToString();
    }
}
