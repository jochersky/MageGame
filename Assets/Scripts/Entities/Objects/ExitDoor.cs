using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public string nextLevel;

    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(nextLevel);
    }
}
