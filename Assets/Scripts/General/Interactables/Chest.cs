using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [Header("Item Info")]
    [SerializeField] private ItemConfig itemConfig;
    [SerializeField] private int count = 1;
    [Header("UI")]
    [SerializeField] private GameObject itemFramePrefab;
    [SerializeField] private Transform itemFrameTransform;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] bool randomSpell = false;
    
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    private GameObject _itemPrefabInstance;
    private GameObject _itemFramePrefabInstance;
    private bool _chestOpened = false;
    private bool _itemTaken;
    
    private readonly int _closed = Animator.StringToHash("ChestClosed");
    private readonly int _open = Animator.StringToHash("ChestOpen");

    private void Start()
    {
        if (randomSpell)
        {
            ChestManager cm = FindAnyObjectByType<ChestManager>();
            itemConfig = cm.GetSpellConfig();
        }
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _itemFramePrefabInstance = Instantiate(itemFramePrefab, itemFrameTransform);
        ItemFrame itemFrame = _itemFramePrefabInstance.GetComponent<ItemFrame>();
        itemFrame.itemFrameIcon.sprite = itemConfig.icon;
        _itemFramePrefabInstance.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_itemTaken && collision.CompareTag("Player")) outline.enabled = true;
        if (_chestOpened) EventBus.Instance.HandleChestOpened(true);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_itemTaken && other.CompareTag("Player")) outline.enabled = false;
        if (_chestOpened) EventBus.Instance.HandleChestOpened(false);
    }

    public void Interact()
    {
        EventBus.Instance.HandleChestOpened(!_chestOpened);
        _animator.CrossFade(_open, 0, 0);
        _itemFramePrefabInstance.SetActive(true);

        if (itemConfig && _chestOpened)
        {
            InventoryManager.Instance.AddItem(itemConfig, count);
            _boxCollider2D.enabled = false;
            _itemFramePrefabInstance.SetActive(false);
            _itemTaken = true;
            outline.enabled = false;
        }
        
        // only let item be added with extra button press
        _chestOpened = true;
    }
}
