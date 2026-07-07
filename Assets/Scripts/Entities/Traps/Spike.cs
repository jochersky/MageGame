using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Spike : MonoBehaviour
{
    public Vector2 directionFired;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TemporaryEffect hitWallEffect;
    [SerializeField] float velocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(velocity * directionFired);
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.CompareTag("Environment"))
        {
            Instantiate(hitWallEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);   
        }
    }
}
