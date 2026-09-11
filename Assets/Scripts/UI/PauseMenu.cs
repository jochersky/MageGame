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
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    private PlayerInput playerInput;
    bool showing = false; 
    void Start()
    {
        StartCoroutine(DelayedStart());
    }
    // because player gets swapped out at start
    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        playerInput = FindAnyObjectByType<PlayerInput>();
        Debug.Log(playerInput.actions[inputEventName]);
        playerInput.actions[inputEventName].performed += Toggle;
    }

    public void Toggle()
    {
        if (showing)
        {
            pauseMenuUI.SetActive(false);
            showing = false;
            //playerInput.SwitchCurrentActionMap(playerInput.defaultActionMap);
        } else
        {
            pauseMenuUI.SetActive(true);
            showing = true;
            //playerInput.SwitchCurrentActionMap("UI");
        }
    }
    public void Toggle(InputAction.CallbackContext context)
    {
        Toggle();
        
    }
    public void OnQuitPressed()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
