using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    private IInteractable _interactable;
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (_interactable == null) return;
        
        _interactable.Interact();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + " entered " + gameObject.name);
        if (collision.gameObject.TryGetComponent<IInteractable>(out IInteractable _interactable))
        {
            _interactable.Interact();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<IInteractable>(out IInteractable _interactable) && _interactable == _interactable)
        {
            _interactable = null;
        }
    }
}
