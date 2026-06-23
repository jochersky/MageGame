using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public Player Player {  get; set; }
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        // Ensure only one instance of the inventory exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Keyboard.current.numpad1Key.wasPressedThisFrame) SaveSystem.Save();
        if (Keyboard.current.numpad2Key.wasPressedThisFrame) SaveSystem.Load();
    }
}
