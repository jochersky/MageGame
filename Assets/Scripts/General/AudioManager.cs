using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    bool playingAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio(AudioClip audio, float delay)
    {
        if (!playingAudio)
        {
            audioSource.clip = audio;
            StartCoroutine(PlayAudioClip(delay));
        }
            
    }

    private IEnumerator PlayAudioClip(float delay)
    {
        playingAudio = true;
        audioSource.PlayOneShot(audioSource.clip);
        yield return new WaitForSeconds(delay);
        playingAudio = false;
    }
}
