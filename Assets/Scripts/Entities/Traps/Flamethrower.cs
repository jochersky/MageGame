using System.Collections;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    bool triggered = false;
    [SerializeField] float triggerDelay = 0.5f;
    [SerializeField] float damageDelay = 0.2f;
    [SerializeField] float duration = 2f;
    [SerializeField] Collider2D hitbox;
    Collider2D player;

    void Start()
    {
        hitbox.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision;
            if (!triggered)
            {
                GetComponentInChildren<AudioSource>().Play();
                StartCoroutine(ThrowFlames());
            }
           
        }
    }



    IEnumerator ThrowFlames()
    {
        triggered = true;
        yield return new WaitForSeconds(triggerDelay);
        hitbox.enabled = true;
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(duration);
        GetComponentInChildren<ParticleSystem>().Stop();
        hitbox.enabled = false;
        triggered = false;
    }

    // should probably be an event
    private void Update()
    {
        // if (firing)
        // {
        //     hitbox.enabled = true;
        // } else
        // {
        //     hitbox.enabled = false;
        // }
    }
}
