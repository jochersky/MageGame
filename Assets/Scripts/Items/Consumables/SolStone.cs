using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class SolStone : MonoBehaviour
{
    [SerializeField] private Color[] colors;
    [SerializeField] private SpriteRenderer stoneSprite;
    [SerializeField] private Light2D light2D;
    [SerializeField] private AudioClip dropSound;
    [SerializeField] private float furthestDropDistance = 20f;
    [SerializeField] private float dropVolume = 0.5f;
    [SerializeField] private float dropPitchMin = 0.8f;
    [SerializeField] private float dropPitchMax = 2f;

    private Vector2 _spawnPos;
    
    private void Start()
    {
        if (colors.Length <= 0) return;

        Color color = colors[Random.Range(0, colors.Length)];
        stoneSprite.color = color;
        light2D.color = color;
        
        _spawnPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        float distance = Vector3.Distance(other.contacts[0].point, _spawnPos);
        float p = Mathf.Min(1, distance / furthestDropDistance);
        float pitch = Mathf.Min(dropPitchMax, dropPitchMin + Random.Range(-0.1f, 0.1f) + p);

        AudioManager.instance.PlayClipAt(dropSound, transform, dropVolume, pitch);
    }
}
