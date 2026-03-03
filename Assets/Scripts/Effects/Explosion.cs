using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Explosion : MonoBehaviour
{
    [SerializeField] private CircleCollider2D tileCollider;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Tilemap>(out Tilemap tilemap))
        {
            // TODO: destroy tiles
        }
    }
}
