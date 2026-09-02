using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public string nextLevel;
    [SerializeField] string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
        {
            StartCoroutine(GoToNextLevel());
        }
    }

    private IEnumerator GoToNextLevel()
    {
        SaveSystem.Save();
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(nextLevel);
    }
}
