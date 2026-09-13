using System;
using NUnit.Framework;
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
    [SerializeField] bool isMainMenu = false;
    [SerializeField] AudioManager audioManager;

    private bool _startAlreadyPressed = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        eventSystem.firstSelectedGameObject = defaultButton;
    }
    // Button Press SFX courtesy of Sonic SoundFX
    public void StartPressed()
    {
        if (_startAlreadyPressed) return;
        
        _startAlreadyPressed = true;
        audioManager.PlayAudio(clickSFX, clickSFX.length);
        if (!isMainMenu) SaveSystem.ResetToStartingSaveState(); 
        SceneManager.LoadScene(gameStartScene);
    }

    public void SettingsPressed()
    {
        audioManager.PlayAudio(clickSFX, clickSFX.length);
        menuButtons.SetActive(false);
        settings.SetActive(true);
    }

    public void BackPressed()
    {
        audioManager.PlayAudio(clickSFX, clickSFX.length);
        menuButtons.SetActive(true);
        settings.SetActive(false);
    }

    public void QuitPressed()
    {
        audioManager.PlayAudio(clickSFX, clickSFX.length);
        Application.Quit();
        Debug.Log("Quitting game...");
    }
    
}
