using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemPrefab;
    
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
        _itemFramePrefabInstance = Instantiate(_item.itemFramePrefab);
        _itemFramePrefabInstance.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + " entered " + gameObject.name);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        
    }

    public void Interact()
    {
        Debug.Log("Interacting with chest");
        _animator.CrossFade(_open, 0, 0);
        _boxCollider2D.enabled = false;
        _itemFramePrefabInstance.SetActive(true);
    }
}
