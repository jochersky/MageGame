using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Landmine : Trap
{
    [SerializeField] TemporaryEffect clickEffect;
    [SerializeField] TemporaryEffect boomEffect;
    [SerializeField] Collider2D explosionHitbox;
    [SerializeField] Collider2D damageHitbox;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] float delay = 0.1f;
    bool triggered = false;
    public override bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(tileCoords) || nonColliderMap.HasTile(tileCoords);
        bool below = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y - 1));
        bool above = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y + 1)) && nonColliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y + 1));
        return !self && below && !above;
    }

    void Start()
    {
        damageHitbox.enabled = false;
        explosionHitbox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Hitbox"))
        {
            Instantiate(clickEffect, transform.position, Quaternion.identity);
            if (!triggered)
            {
                triggered = true;
                StartCoroutine(Explode());
            }
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(delay);
        Instantiate(boomEffect, transform.position, Quaternion.identity);
        damageHitbox.enabled = true;
        explosionHitbox.enabled = true;
        sprite.enabled = false;
        //yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
