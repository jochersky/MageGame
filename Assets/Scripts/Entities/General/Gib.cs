using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Gib : MonoBehaviour
{
    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        
        _health.OnDeath += () => StartCoroutine(DestroyProcedure());
    }

    IEnumerator DestroyProcedure()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
