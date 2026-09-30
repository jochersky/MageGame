using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FalseFloor : MonoBehaviour
{
    [SerializeField] AudioClip breakingSound;
    [SerializeField] float audioDelayForVolumeControl = 0.1f;
    [SerializeField] TemporaryEffect breakEffect;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Hurtbox hurtbox;
    string playerFeetTag = "Stomp";
    Tilemap colliderTilemap;
    float durability = 0.5f;
    bool crumbling = false;

    void Start()
    {
        hurtbox.OnDamageTaken += OnHit;
        MapGenerator mapGenerator = FindAnyObjectByType<MapGenerator>();
        if (mapGenerator != null)
        {
            colliderTilemap = FindAnyObjectByType<MapGenerator>().getColliderMap();
        } else
        {
            colliderTilemap = FindAnyObjectByType<TilemapCollider2D>().GetComponent<Tilemap>();
        }
    }

    void Update()
    {
        if (crumbling)
        {
            durability -= Time.deltaTime;
            // if full or half broken, add cracks and dust effect, crack SFX
            if (durability <= 0.0f)
            {
                crumbling = false;
                Break();
            }
            }
        
    }

    private void OnHit(DamageProperties damageProperties)
    {
        durability -= damageProperties.amount;
        if (durability <= 0.0f)
        {
            crumbling = false;
            Break();
        }
        // play effects
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(playerFeetTag))
        {
            crumbling = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(playerFeetTag))
        {
            crumbling = false;
        }
    }

    private void Break()
    {
        spriteRenderer.enabled = false;
        Vector3 worldPos = transform.position;
        Vector3Int pos = colliderTilemap.WorldToCell(worldPos);
        Instantiate(breakEffect, transform.position, quaternion.identity);
        AudioManager.instance.PlayAudio(breakingSound, audioDelayForVolumeControl);
        if (colliderTilemap.GetTile(pos))
        {
            // I guess this 'destroys' the object
            colliderTilemap.SetTile(pos, null);
        }
        
        //EventBus.Instance.HandleTileMapChanged();
        //Destroy(gameObject);
    }

}
