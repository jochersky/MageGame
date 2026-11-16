using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlSchemeType
{
    KEYBOARD,
    GAMEPAD
}

public class InputIconDictionary
{
    private readonly Dictionary<string, Sprite> _keyboardIcons = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, Sprite> _gamepadIcons = new Dictionary<string, Sprite>();

    public InputIconDictionary()
    {
        _keyboardIcons["UseConsumable"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_i");
        _keyboardIcons["SwitchConsumable"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_u");
        _keyboardIcons["Spell 1"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_j");
        _keyboardIcons["Spell 2"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_l");
        _keyboardIcons["Attack"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_k");
        _keyboardIcons["Cancel"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_q");
        _keyboardIcons["Submit"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_e");
        _keyboardIcons["Interact"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_e");
        _keyboardIcons["Dodge"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_shift");
        _keyboardIcons["Jump"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_space");
        
        _gamepadIcons["UseConsumable"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/lb_outline");
        _gamepadIcons["SwitchConsumable"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/dpad_north");
        _gamepadIcons["Spell 1"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/lt_outline");
        _gamepadIcons["Spell 2"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/rt_outline");
        _gamepadIcons["Attack"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/rb_outline");
        _gamepadIcons["Cancel"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/gp_east_outline");
        _gamepadIcons["Submit"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/gp_south_outline");
        _gamepadIcons["Interact"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/gp_west_outline");
        _gamepadIcons["Dodge"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/gp_east_outline");
        _gamepadIcons["Jump"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/gp_south_outline");
    }

    public Sprite GetIcon(ControlSchemeType type, string inputActionName)
    {
        Dictionary<string, Sprite> icons = type == ControlSchemeType.KEYBOARD ? _keyboardIcons : _gamepadIcons;
        // Debug.Log($"input action: {inputActionName}");
        if (inputActionName == null) return null;
        return icons.GetValueOrDefault(inputActionName);
    }
}
