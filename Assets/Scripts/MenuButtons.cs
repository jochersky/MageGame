using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    [SerializeField] String gameStartScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void StartPressed()
    {
        SceneManager.LoadScene(gameStartScene);
    }

    public void QuitPressed()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }

}
