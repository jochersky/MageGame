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
    [SerializeField] GameObject item;
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
        if (purchased || !_inRange || mc.GetMoney() < price)
        {
            return;
        }
        mc.AddMoney(-price);
        purchased = true;
        Instantiate(item, transform.position, transform.rotation);
        display.enabled = false;
        priceObject.SetActive(false);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        _inRange = true;
        outline.enabled = true;
        if (!purchased)
        {
            priceObject.SetActive(true);
        }
        print("ENTER");
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _inRange = false;
        outline.enabled = false;
        if (!purchased)
        {
            priceObject.SetActive(false);
        }
        print("LEAVE");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
