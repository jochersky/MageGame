using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private PassiveSpellAffects passiveSpellAffects;
    private ILockedInteractable _lockedInteractable;
    private IInteractable _interactable;
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;

        if (_interactable != null) _interactable.Interact();
        else if (_lockedInteractable != null) _lockedInteractable.Interact(passiveSpellAffects);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IInteractable interactable))
        {
            _interactable = interactable;
        }
        else if (collision.gameObject.TryGetComponent(out ILockedInteractable lockedInteractable))
        {
            _lockedInteractable = lockedInteractable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out IInteractable interactable) && interactable == _interactable)
        {
            _interactable = null;
        }
        else if (other.gameObject.TryGetComponent(out ILockedInteractable lockedInteractable) && lockedInteractable == _lockedInteractable)
        {
            _lockedInteractable = null;
        }
    }
}
