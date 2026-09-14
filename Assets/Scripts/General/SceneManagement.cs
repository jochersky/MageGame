using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] AudioClip levelMusic;
    [SerializeField] AudioManager audioManagerPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        if (audioManager == null)
        {
            audioManager = Instantiate(audioManagerPrefab);
        }
        audioManager.ChangeMusic(levelMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
