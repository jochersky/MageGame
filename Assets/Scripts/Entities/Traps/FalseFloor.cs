using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FalseFloor : MonoBehaviour
{
    [SerializeField] AudioClip breakingSound;
    [SerializeField] float audioDelayForVolumeControl = 0.1f;
    [SerializeField] TemporaryEffect breakEffect;
    [SerializeField] SpriteRenderer spriteRenderer;
    Tilemap colliderTilemap;
    bool triggered = false;

    void Start()
    {
        MapGenerator mapGenerator = FindAnyObjectByType<MapGenerator>();
        if (mapGenerator != null)
        {
            colliderTilemap = FindAnyObjectByType<MapGenerator>().getColliderMap();
        } else
        {
            colliderTilemap = FindAnyObjectByType<TilemapCollider2D>().GetComponent<Tilemap>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !triggered)
        {
            triggered = true;
            spriteRenderer.enabled = false;
            Vector3 worldPos = transform.position;
            Vector3Int pos = colliderTilemap.WorldToCell(worldPos);
            if (colliderTilemap.GetTile(pos))
            {
                colliderTilemap.SetTile(pos, null);
            }
            Instantiate(breakEffect, transform.position, quaternion.identity);
            AudioManager.instance.PlayAudio(breakingSound, audioDelayForVolumeControl);
            //EventBus.Instance.HandleTileMapChanged();
            Destroy(gameObject);
        }
    }

}
