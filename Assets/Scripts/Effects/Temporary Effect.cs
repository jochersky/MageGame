using System.Collections;
using UnityEngine;

public class TemporaryEffect : MonoBehaviour
{
    [SerializeField] float duration = 1f;
    [SerializeField] ParticleSystem vfx;
    [SerializeField] AudioSource sfx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(PlayEffectThenDie());
    }

    IEnumerator PlayEffectThenDie()
    {
        vfx.Play();
        sfx.Play();
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
