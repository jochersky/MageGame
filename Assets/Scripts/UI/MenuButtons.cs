using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] string gameStartScene;
    [SerializeField] AudioClip clickSFX;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] GameObject defaultButton;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject menuButtons;
    AudioSource audioSource;

    private bool _startAlreadyPressed = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        eventSystem.firstSelectedGameObject = defaultButton;
    }
    // TODO: This SFX system is bugged and will have problems later
    // Button Press SFX courtesy of Sonic SoundFX
    public void StartPressed()
    {
        if (_startAlreadyPressed) return;
        
        _startAlreadyPressed = true;
        audioSource.PlayOneShot(clickSFX);
        SaveSystem.ResetToStartingSaveState(); 
        SceneManager.LoadScene(gameStartScene);
    }

    public void SettingsPressed()
    {
        audioSource.PlayOneShot(clickSFX);
        menuButtons.SetActive(false);
        settings.SetActive(true);
    }

    public void BackPressed()
    {
        audioSource.PlayOneShot(clickSFX);
        menuButtons.SetActive(true);
        settings.SetActive(false);
    }

    public void QuitPressed()
    {
        audioSource.PlayOneShot(clickSFX);
        Application.Quit();
        Debug.Log("Quitting game...");
    }
    
}
