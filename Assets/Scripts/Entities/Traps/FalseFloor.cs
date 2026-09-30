using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FalseFloor : MonoBehaviour
{
    [SerializeField] AudioClip breakingSound;
    [SerializeField] AudioClip[] crackingSounds;
    [SerializeField] float audioDelayForVolumeControl = 0.1f;
    [SerializeField] TemporaryEffect breakEffect;
    [SerializeField] TemporaryEffect crackEffect;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite crackedFloorSprite;
    [SerializeField] Hurtbox hurtbox;
    readonly float crackSFXVolume = 0.1f;
    readonly string playerFeetTag = "Stomp";
    Tilemap colliderTilemap;
    float maxDurability;
    float durability = 0.5f;
    bool crumbling = false;

    void Start()
    {
        maxDurability = durability;
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
            if (durability == maxDurability || durability == maxDurability / 1.8f)
            {
                spriteRenderer.sprite = crackedFloorSprite;
                if (AudioManager.instance != null)
                    AudioManager.instance.PlayRandomClipFromAt(crackingSounds, transform, crackSFXVolume);
                else Debug.Log("No AudioManager found");
                Instantiate(crackEffect, transform.position, quaternion.identity);
            }
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
        } else
        {
            spriteRenderer.sprite = crackedFloorSprite;
            if (AudioManager.instance != null)
                AudioManager.instance.PlayRandomClipFromAt(crackingSounds, transform, crackSFXVolume);
            else Debug.Log("No AudioManager found");
            Instantiate(crackEffect, transform.position, quaternion.identity);
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
        if (AudioManager.instance != null)
            AudioManager.instance.PlayAudio(breakingSound, audioDelayForVolumeControl);
        else Debug.Log("No AudioManager found");
        if (colliderTilemap.GetTile(pos))
        {
            // I guess this 'destroys' the object
            colliderTilemap.SetTile(pos, null);
        }
        
        //EventBus.Instance.HandleTileMapChanged();
        //Destroy(gameObject);
    }

}
