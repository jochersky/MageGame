using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Gib : MonoBehaviour, ILockedInteractable
{
    [Header("References")]
    [SerializeField] private GameObject objectToDestroy;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ParticleSystem particleSystem;
    [Header("Properties")]
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Color color;
    
    private Health _health;

    private void Start()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
        spriteRenderer.color = color;
        
        _health = GetComponent<Health>();
        
        _health.OnDeath += () => StartCoroutine(DestroyProcedure());
    }

    IEnumerator DestroyProcedure()
    {
        // particleSystem.
        particleSystem.Play();
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(particleSystem.main.duration);
        Destroy(objectToDestroy);
    }

    public void Interact(PassiveSpellAffects affects)
    {
        if (affects.canDevour)
        {
            GameManager.Instance.PlayerHealth.Heal(1);
            StartCoroutine(DestroyProcedure());
        }
    }
}
