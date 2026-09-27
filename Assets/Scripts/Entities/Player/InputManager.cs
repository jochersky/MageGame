using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    private InputActionMap _playerInputMap;
    
    public delegate void PlayerPressedInventory(bool pressed);
    public event PlayerPressedInventory OnPlayerPressedInventory;
    public delegate void PlayerPressedCancel();
    public event PlayerPressedCancel OnPlayerPressedCancel;
    public delegate void PlayerPressedPause();
    public event PlayerPressedPause OnPlayerPressedPause;
    
    private bool _inputDisabled;

    private void Start()
    {
        // TODO: investigate why this event isn't firing
        // playerInput.onControlsChanged += 
        
        _playerInputMap = playerInput.actions.actionMaps[0];
    }
    
    public void OnInventoryPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;

        _inputDisabled = !_inputDisabled;
        OnPlayerPressedInventory?.Invoke(_inputDisabled);
        
        // Disable all actions besides the ability to open/close inventory 
        // so that the player cannot move while it is open
        foreach (InputAction action in _playerInputMap.actions)
        {
            if (action.name.Contains("UI")) continue;
            
            if (action.name != "Inventory" && action.name != "Cancel")
            {
                if (_inputDisabled) action.Disable();
                else action.Enable();
            }
        }
    }
    
    public void OnCancelPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;
        
        OnPlayerPressedCancel?.Invoke();
    }

    public void OnPausePressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;
        
        OnPlayerPressedPause?.Invoke();
    }
}
