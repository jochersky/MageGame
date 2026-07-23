using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Flamethrower : MonoBehaviour
{
    bool triggered = false;
    [SerializeField] float triggerDelay = 0.5f;
    [SerializeField] float damageDelay = 0.2f;
    [SerializeField] float duration = 2f;
    [SerializeField] GameObject hitboxPivot;
    [SerializeField] GameObject triggerboxPivot;
    [SerializeField] Collider2D fireHitbox;
    [SerializeField] LayerMask fireLayermask;
    [SerializeField] AudioSource sfx;
    [SerializeField] ParticleSystem vfx;
    bool extendingHitbox = false;
    float extension = 0.0f;
    float extensionTimer = 0.0f;
    float length = 0.0f;

    void Start()
    {
        fireHitbox.enabled = false;
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 100, fireLayermask);
        Vector2 newPosition = hit.point;
        length = hitboxPivot.transform.position.y - newPosition.y;
        triggerboxPivot.transform.localScale = new Vector3(1, length, 1);
        ParticleSystem.MainModule vfxMain = vfx.main;
        vfxMain.startLifetimeMultiplier = length / 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!triggered)
            {
                sfx.Play();
                StartCoroutine(ThrowFlames());
            }
           
        }
    }



    IEnumerator ThrowFlames()
    {
        triggered = true;
        yield return new WaitForSeconds(triggerDelay);
        extendingHitbox = true;
        //fireHitbox.enabled = true;
        vfx.Play();
        yield return new WaitForSeconds(duration);
        vfx.Stop();
        fireHitbox.enabled = false;
        triggered = false;
    }

    // should probably be an event
    private void Update()
    {
        if (extendingHitbox)
        {
            if (extensionTimer < vfx.main.startLifetime.constant) //vfx.main.startLifetime.constant
            {
                extensionTimer += Time.deltaTime;
                extension = length * extensionTimer / vfx.main.startLifetime.constant;
                hitboxPivot.transform.localScale = new Vector3(1, extension, 1);
                if (fireHitbox.enabled == false)
                {
                    fireHitbox.enabled = true;
                }
            } else
            {
                extendingHitbox = false;
                extension = 0.0f;
                extensionTimer = 0.0f;
            }
        }
    }
}
