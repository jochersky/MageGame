using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private Collider2D hitboxCollider;
    [SerializeField, Range(0, 10)] private float explodeTime = 1;
    
    private void Start()
    {
        hitboxCollider.enabled = false;
        StartCoroutine(InitiateExplode());
    }

    private void Explode()
    {
        Destroy(gameObject);
    }

    private IEnumerator InitiateExplode()
    {
        float timer = 0f;
        while (timer < explodeTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        hitboxCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        Explode();
    }
}
