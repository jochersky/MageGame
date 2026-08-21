using System.Collections;
using UnityEngine;

public class ExplodeOnDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private DamageFlash explosionFlash;
    [SerializeField] private GameObject bombPrefab;
    [Header("Properties")]
    [SerializeField] private float timeUntilExplosion;

    private void Start()
    {
        health.OnDeath += StartExplosionSequence;
        explosionFlash.OnDamageFlashComplete += () => { explosionFlash.StartFlash(); };
    }

    public void StartExplosionSequence()
    {
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        yield return new WaitForSeconds(0.1f);
        explosionFlash.StartFlash();
        
        bombPrefab.GetComponent<Bomb>().ExplodeTime = timeUntilExplosion;
        Instantiate(bombPrefab, transform);

        yield return new WaitForSeconds(timeUntilExplosion);
        yield return new WaitForSeconds(0.1f);
        
        Destroy(gameObject);
    }
}
