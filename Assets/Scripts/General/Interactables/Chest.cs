using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemFrameTransform;
    
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    private GameObject _itemPrefabInstance;
    private Item _item;
    private GameObject _itemFramePrefabInstance;
    
    private readonly int _closed = Animator.StringToHash("ChestClosed");
    private readonly int _open = Animator.StringToHash("ChestOpen");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _itemPrefabInstance = Instantiate(itemPrefab);
        _item = _itemPrefabInstance.GetComponent<Item>();
        _itemFramePrefabInstance = Instantiate(_item.itemFramePrefab, itemFrameTransform);
        _itemFramePrefabInstance.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
    }

    public void Interact()
    {
        _animator.CrossFade(_open, 0, 0);
        _boxCollider2D.enabled = false;
        _itemFramePrefabInstance.SetActive(true);
        if (_itemPrefabInstance.TryGetComponent<Consumable>(out Consumable consumable))
        {
            InventoryManager.Instance.UpdateConsumable(consumable.consumableType, consumable.count);
        }
        // TODO
        else if (_itemPrefabInstance.TryGetComponent<ActiveSpell>(out ActiveSpell spell))
        {
            InventoryManager.Instance.AddSpell(spell);
        }
        else if (_itemPrefabInstance.TryGetComponent<PassiveSpell>(out PassiveSpell passiveSpell))
        {
            InventoryManager.Instance.AddPassiveSpell(passiveSpell);
        }
    }
}
