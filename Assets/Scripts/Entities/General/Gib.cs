using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Gib : MonoBehaviour, ILockedInteractable
{
    [SerializeField] private GameObject objectToDestroy;
    private Health _health;

    private void Start()
    {
        _health = GetComponent<Health>();
        
        _health.OnDeath += () => StartCoroutine(DestroyProcedure());
    }

    IEnumerator DestroyProcedure()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(objectToDestroy);
    }

    public void Interact(PassiveSpellAffects affects)
    {
        Debug.Log("Interacting with");
        if (affects.canDevour)
        {
            GameManager.Instance.PlayerHealth.Heal(1);
            StartCoroutine(DestroyProcedure());
        }
    }
}
