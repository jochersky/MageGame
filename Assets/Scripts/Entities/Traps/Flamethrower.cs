using System.Collections;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    bool isInRange = false;
    bool firing = false;
    float triggerDelay = 0.5f;
    float duration = 2f;
    Collider2D player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player = collision;
            isInRange = true;
            if (!firing)
            {
                GetComponentInChildren<AudioSource>().Play();
                StartCoroutine(throwFlames());
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isInRange = false;
        }
    }

    IEnumerator throwFlames()
    {
        yield return new WaitForSeconds(triggerDelay);
        firing = true;
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(duration);
        GetComponentInChildren<ParticleSystem>().Stop();
        firing = false;
    }

    private void Update()
    {
        if (isInRange && firing)
        {
            player.GetComponentInChildren<SpriteRenderer>().color = Color.orange;
        }
        // } else
        // {
        //     player.GetComponentInChildren<SpriteRenderer>().color = Color.clear;
        // }
    }
}
