using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] string menuSceneName;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] string inputEventName;
    [SerializeField] string closeEventName;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    private PlayerInput playerInput;
    bool showing = false; 
    void Awake()
    {
        FindAnyObjectByType<MapGenerator>().OnPlayerPlaced += SetupInput;
    }

    void SetupInput(GameObject player)
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerInput.actions[inputEventName].performed += Toggle;
        playerInput.actions[closeEventName].performed += Toggle;
    }

    public void Toggle()
    {
        if (showing)
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            showing = false;
            playerInput.currentActionMap.Disable();
            playerInput.SwitchCurrentActionMap(playerInput.defaultActionMap);
            playerInput.currentActionMap.Enable();
        } else
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
            showing = true;
            playerInput.currentActionMap.Disable();
            playerInput.SwitchCurrentActionMap("UI");
            playerInput.currentActionMap.Enable();
        }
    }
    public void Toggle(InputAction.CallbackContext context)
    {
        Toggle();
        
    }
    public void OnQuitPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
