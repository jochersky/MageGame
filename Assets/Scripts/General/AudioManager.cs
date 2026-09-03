using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource audioSourcePrefab;
    [SerializeField] AudioClip background_music;
    bool playingAudio;
    
    void Awake()
    {
        // set up singleton
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomClipFromAt(AudioClip[] audioClips, Transform location, float volume)
    {
        PlayClipAt(audioClips[Random.Range(0, audioClips.Length)], location, volume);
    }

    public void PlayClipAt(AudioClip audioClip, Transform location, float volume)
    {
        AudioSource audioSrcInstance = Instantiate(audioSourcePrefab, location.position, Quaternion.identity);
        audioSrcInstance.clip = audioClip;
        audioSrcInstance.volume = volume;
        audioSrcInstance.Play();
        Destroy(audioSrcInstance, audioClip.length);
    }

    public void PlayAudio(AudioClip audio, float duration)
    {
        if (!playingAudio)
        {
            audioSource.clip = audio;
            StartCoroutine(PlayAudioClip(duration));
        }
            
    }

    private IEnumerator PlayAudioClip(float duration)
    {
        playingAudio = true;
        audioSource.PlayOneShot(audioSource.clip);
        yield return new WaitForSeconds(duration);
        playingAudio = false;
    }
}
