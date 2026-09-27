using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] GameObject backButtonGO;
    [SerializeField] GameObject unpausedSelectedGO;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] bool debug;
    private PlayerInput playerInput;
    bool showing = false; 
    
    void Start()
    {
        if (!debug) FindAnyObjectByType<MapGenerator>().OnPlayerPlaced += SetupInput;
        else SetupInput(GameManager.Instance.Player.gameObject);
    }

    void SetupInput(GameObject player)
    {
        playerInput = player.GetComponent<PlayerInput>();
        playerInput.actions[inputEventName].performed += Toggle;
        playerInput.actions[closeEventName].performed += Toggle;
    }

    public void Toggle()
    {
        Debug.Log(playerInput);
        if (showing)
        {
            eventSystem.SetSelectedGameObject(unpausedSelectedGO);
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            showing = false;
            playerInput.currentActionMap.Disable();
            playerInput.SwitchCurrentActionMap(playerInput.defaultActionMap);
            playerInput.currentActionMap.Enable();
        } else
        {
            eventSystem.SetSelectedGameObject(backButtonGO);
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
