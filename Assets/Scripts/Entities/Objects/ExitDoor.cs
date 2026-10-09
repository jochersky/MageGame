using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour, IInteractable
{
    public string nextLevel;
    [SerializeField] string playerTag = "Player";
    [SerializeField] GameObject outline;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag)) outline.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag)) outline.SetActive(false);
    }
    
    public void Interact()
    {
        StartCoroutine(GoToNextLevel());
    }

    private IEnumerator GoToNextLevel()
    {
        SaveSystem.Save();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextLevel);
    }
}
