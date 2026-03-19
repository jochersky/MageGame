using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] string gameStartScene;
    [SerializeField] AudioClip clickSFX;
    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // TODO: This SFX system is bugged and will have problems later
    // Button Press SFX courtesy of Sonic SoundFX
    public void StartPressed()
    {
        audioSource.PlayOneShot(clickSFX);
        SceneManager.LoadScene(gameStartScene);
    }

    public void SettingsPressed()
    {
        audioSource.PlayOneShot(clickSFX);
        Debug.Log("Loading settings...");
    }

    public void QuitPressed()
    {
        audioSource.PlayOneShot(clickSFX);
        Application.Quit();
        Debug.Log("Quitting game...");
    }

}
