using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private int count = 1;
    [Header("UI")]
    [SerializeField] private GameObject itemFramePrefab;
    [SerializeField] private Transform itemFrameTransform;
    
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    private GameObject _itemPrefabInstance;
    private GameObject _itemFramePrefabInstance;
    
    private readonly int _closed = Animator.StringToHash("ChestClosed");
    private readonly int _open = Animator.StringToHash("ChestOpen");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _itemFramePrefabInstance = Instantiate(itemFramePrefab, itemFrameTransform);
        ItemFrame itemFrame = _itemFramePrefabInstance.GetComponent<ItemFrame>();
        itemFrame.itemFrameIcon.sprite = itemConfig.icon;
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

        if (itemConfig) InventoryManager.Instance.AddItem(itemConfig, count);
    }
}
