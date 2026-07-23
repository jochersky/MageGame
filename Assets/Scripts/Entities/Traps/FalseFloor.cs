using UnityEngine;

public class FalseFloor : MonoBehaviour
{
    [SerializeField] AudioClip breakingSound;
    [SerializeField] float audioDelayForVolumeControl = 0.1f;
    private AudioManager audioManager;
    bool triggered = false;

    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !triggered)
        {
            triggered = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponentInChildren<ParticleSystem>().Play();
            audioManager.PlayAudio(breakingSound, audioDelayForVolumeControl);
        }
    }

}
