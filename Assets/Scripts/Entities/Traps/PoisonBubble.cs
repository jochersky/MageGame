using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PoisonBubble : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 50f;
    [SerializeField] TemporaryEffect effects;
    [SerializeField] AudioClip[] popSounds;
    [SerializeField] float popVolume;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger || collision.CompareTag("Hitbox"))
        {
            Instantiate(effects, transform.position, quaternion.identity);
            AudioManager.instance.PlayRandomClipFromAt(popSounds, transform, popVolume);
            Destroy(gameObject);
        } 
    }
}
