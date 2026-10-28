using System.Linq;
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

    public delegate void PlayerPressedInteract();
    public event PlayerPressedInteract OnPlayerPressedInteract;
    
    private bool _inputDisabled;
    private string[] _inventoryMenuActiveActions = { "Inventory", "Cancel" };
    
    // Singleton instance
    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of the input manager exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
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
        
        ToggleInputsBesides(_inventoryMenuActiveActions, _inputDisabled);
        
        // Disable all actions besides the ability to open/close inventory 
        // so that the player cannot move while it is open
        // foreach (InputAction action in _playerInputMap.actions)
        // {
        //     if (action.name.Contains("UI")) continue;
        //     
        //     if (action.name != "Inventory" && action.name != "Cancel")
        //     {
        //         if (_inputDisabled) action.Disable();
        //         else action.Enable();
        //     }
        // }
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

    public void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled) return;
        
        OnPlayerPressedInteract?.Invoke();
    }

    // Toggles all inputs besides the inputs with the names provided in activeActions.
    // Use-case is primarily in menus.
    public void ToggleInputsBesides(string[] activeActions, bool disable)
    {
        foreach (InputAction action in _playerInputMap.actions)
        {
            // if (action.name.Contains("UI")) continue;
            
            if (!activeActions.Contains(action.name))
            {
                if (disable) action.Disable();
                else action.Enable();
            }
        }
    }
}
