using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class SceneManagement : MonoBehaviour
{
    private AudioManager audioManager;
    [SerializeField] AudioClip levelMusic;
    [SerializeField] AudioManager audioManagerPrefab;
    
    public static SceneManagement Instance { get; private set; }
    
    private void Awake()
    {
        // ensure only one instance of the inventory manager exists globally
        if (Instance && Instance != this)
        {
            Destroy(this);
            Debug.Log("Duplicate scene destruction");
            return;
        }
        Instance = this;
    }
    
    // void Start()
    // {
    //     audioManager = FindFirstObjectByType<AudioManager>();
    //     if (audioManager == null)
    //     {
    //         audioManager = Instantiate(audioManagerPrefab);
    //     }
    //     audioManager.ChangeMusic(levelMusic);
    // }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
 
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void SwapScene(string sceneName)
    {
        Debug.Log($"swapping to {sceneName}");
        Instance.StartCoroutine(Instance.FadeOutThenChangeScene(sceneName));
    }

    private IEnumerator FadeOutThenChangeScene(string sceneName)
    {
        SceneFadeManager.Instance.StartFadingOut();
        
        while (SceneFadeManager.Instance.IsFadingOut) 
        {
            yield return null;
        }
        
        SceneManager.LoadScene(sceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneFadeManager.Instance.StartFadingIn();
    }
}
