using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputIcons : MonoBehaviour
{
    [SerializeField] private Image[] inputImages;
    [SerializeField] private string[] actionNames;
    [SerializeField] private ControlSchemeType defaultControlScheme;
    
    // Stores action names when a unique input requires a name to change
    private string[] _updatedActionNames;
    
    private ControlSchemeType _currentControlScheme;
    
    private void Start()
    {
        _updatedActionNames = new string[actionNames.Length];
        for (int i = 0; i < _updatedActionNames.Length; i++)
        {
            _updatedActionNames[i] = actionNames[i];
        }

        InputManager.Instance.OnSwitchedControlSchemes += UpdateIcons;
        
        // Update with initially set control scheme
        UpdateIcons(defaultControlScheme);
    }

    private void UpdateIcons(ControlSchemeType controlScheme)
    {
        if (inputImages.Length != actionNames.Length)
            throw new System.Exception("Input images length doesn't match input actions length.");

        _currentControlScheme = controlScheme;
        
        for (int i = 0; i < inputImages.Length; i++)
        {
            Image image = inputImages[i];
            string inputActionName = _updatedActionNames[i];
            image.sprite = InputManager.Instance.GetInputIcon(controlScheme, inputActionName);
        }
    }

    // Use for when instances when you want to update action in response to player action,
    // e.g. player equips double jump spell.
    public void UpdateActionName(string oldActionName, string newActionName)
    {
        for (int i = 0; i < _updatedActionNames.Length; i++)
        {
            if (_updatedActionNames[i] == oldActionName) _updatedActionNames[i] = newActionName;
        }
        UpdateIcons(_currentControlScheme);
    }

    // Used for instances when you want to revert action to its original value,
    // e.g. player equips standard spell over double jump.
    public void RevertActionName(string actionName)
    {
        for (int i = 0; i < actionNames.Length; i++)
        {
            if (actionNames[i] == actionName)
            {
                _updatedActionNames[i] = actionNames[i];
            }
        }
        UpdateIcons(_currentControlScheme);
    }
}
