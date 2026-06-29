using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Sellable : MonoBehaviour
{
    [SerializeField] int price;
    [SerializeField] GameObject priceObject;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] SpriteRenderer outline;
    MoneyCounter mc;
    bool purchased = false;
    bool _inRange = false;
    [SerializeField] SpriteRenderer display;
    [SerializeField] ItemConfig item;
    [SerializeField] int count;
    private PlayerInput _input;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = FindAnyObjectByType<PlayerInput>();
        _input.actions["Interact"].performed += OnInteract;
        mc = FindFirstObjectByType<MoneyCounter>();
        priceText.text = price.ToString();
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        if (_inRange)
        {
            Purchase();
        }
    }

    void Purchase()
    {
        if (purchased || !_inRange || InventoryManager.Instance.GetMoneyCount() < price)
        {
            return;
        }
        InventoryManager.Instance.UpdateMoney(-price);
        purchased = true;
        if (item) InventoryManager.Instance.AddItem(item, count);
        display.enabled = false;
        priceObject.SetActive(false);
        outline.enabled = false;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (!purchased)
        {
            _inRange = true;
            outline.enabled = true;
            priceObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        outline.enabled = false;
        _inRange = false;
        if (!purchased)
        {
            priceObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
