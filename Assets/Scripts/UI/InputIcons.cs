using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputIcons : MonoBehaviour
{
    [SerializeField] private Image[] inputImages;
    [SerializeField] private InputActionReference[] inputActions;
    [SerializeField] private ControlSchemeType defaultControlScheme;
    InputIconDictionary _inputIconDictionary;

    private void Awake()
    {
        _inputIconDictionary = new InputIconDictionary();
    }
    
    private void Start()
    {
        InputManager.Instance.OnSwitchedControlSchemes += UpdateIcons;
        
        // Update with initially set control scheme
        UpdateIcons(defaultControlScheme);
    }

    private void UpdateIcons(ControlSchemeType controlScheme)
    {
        if (inputImages.Length != inputActions.Length)
            throw new System.Exception("Input images length doesn't match input actions length.");

        for (int i = 0; i < inputImages.Length; i++)
        {
            Image image = inputImages[i];
            InputAction inputAction = inputActions[i];
            image.sprite = _inputIconDictionary.GetIcon(controlScheme, inputAction);
        }
    }
}
