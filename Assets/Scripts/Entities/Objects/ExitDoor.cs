using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public string nextLevel;
    [SerializeField] string playerTag = "Player";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerTag))
            SceneManager.LoadScene(nextLevel);
    }
}
