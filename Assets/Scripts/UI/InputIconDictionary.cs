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
        _keyboardIcons["Spell 1"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_j");
        _keyboardIcons["Spell 2"] = Resources.Load<Sprite>("Data/ControlIcons/Keyboard/keyboard_l");
        
        _gamepadIcons["UseConsumable"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/lb_outline");
        _gamepadIcons["Spell 1"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/lt_outline");
        _gamepadIcons["Spell 2"] = Resources.Load<Sprite>("Data/ControlIcons/Gamepad/rt_outline");
    }

    public Sprite GetIcon(ControlSchemeType type, InputAction action)
    {
        Dictionary<string, Sprite> icons = type == ControlSchemeType.KEYBOARD ? _keyboardIcons : _gamepadIcons;
        return icons.GetValueOrDefault(action.name);
    }
}
