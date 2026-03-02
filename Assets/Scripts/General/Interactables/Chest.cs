using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private Animator _animator;
    private BoxCollider2D _boxCollider2D;
    
    private readonly int _closed = Animator.StringToHash("ChestClosed");
    private readonly int _open = Animator.StringToHash("ChestOpen");

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
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
    }
}
