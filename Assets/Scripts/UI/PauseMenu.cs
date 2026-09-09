using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    
    [SerializeField] string menuSceneName;
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    MixerManager mixerManager;
    PlayerInput playerInput;
    void Awake()
    {
        mixerManager = FindFirstObjectByType<MixerManager>();
        if (mixerManager != null)
        {
            masterSlider.onValueChanged.AddListener(mixerManager.SetMasterVolume);
            musicSlider.onValueChanged.AddListener(mixerManager.SetMasterVolume);
            sfxSlider.onValueChanged.AddListener(mixerManager.SetMasterVolume);
        } else
        {
            Debug.Log("Pause Menu could not find MixerManager!");
        }
        playerInput = FindFirstObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            //playerInput.uiInputModule.
        } else
        {
            Debug.Log("Pause Menu could not find Player Input!");
        }
        
    }
    private void Show()
    {
        pauseMenuUI.SetActive(false);
    }
    public void OnBackPressed()
    {
        pauseMenuUI.SetActive(false);
    }
    public void OnQuitPressed()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
