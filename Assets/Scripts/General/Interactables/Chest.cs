using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private BoxCollider2D _boxCollider2D;

    private void Start()
    {
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
    }
}
