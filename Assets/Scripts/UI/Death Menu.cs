using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    [SerializeField] float delay = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health playerHealth = FindAnyObjectByType<PlayerStateMachine>().GetComponent<Health>();
        playerHealth.OnDeath += OnPlayerDeath;
    }
    
    private void OnPlayerDeath()
    {
        StartCoroutine(MenuCoroutine());
    }

    private IEnumerator MenuCoroutine()
    {
        yield return new WaitForSeconds(delay);
        menu.SetActive(true);
    }

}
