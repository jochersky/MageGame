using System;
using System.Linq;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class Sellable : MonoBehaviour
{
    [SerializeField] int maxPrice;
    int price = 99;
    [SerializeField] GameObject priceObject;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] SpriteRenderer outline;
    MoneyCounter mc;
    bool purchased = false;
    bool _inRange = false;
    [SerializeField] SpriteRenderer display;
    [SerializeField] ItemConfig item;
    [SerializeField] int count;
    [SerializeField] string configPath;
    private PlayerInput _input;
    private System.Random randy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = FindAnyObjectByType<PlayerInput>();
        _input.actions["Interact"].performed += OnInteract;
        mc = FindFirstObjectByType<MoneyCounter>();
        randy = new System.Random();
        GenerateSellable();
        display.sprite = item.icon;
        price = randy.Next(0, maxPrice);
        priceText.text = price.ToString();
    }

    // probably inefficent for each sellable to do this
    void GenerateSellable()
    {
        ItemConfig[] sellables = Resources.LoadAll<ItemConfig>(configPath);
        //sellables.Concat(Resources.LoadAll<ItemConfig>(spellConfigPath)).ToArray();
        item = sellables[randy.Next(0, sellables.Length)];
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
        if (item != null)
        {
            InventoryManager.Instance.AddItem(item as ItemConfig, count);
        }
        display.enabled = false;
        priceObject.SetActive(false);
        outline.enabled = false;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player") && !purchased)
        {
            _inRange = true;
            outline.enabled = true;
            priceObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            outline.enabled = false;
            _inRange = false;
            if (!purchased)
            {
                priceObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
