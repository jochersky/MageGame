using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Portal : Trap
{
    // Manually assign which portal this portal will be paired with
    public Portal pair;
    [SerializeField] Animator animator;
    [SerializeField] float cooldown;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    private bool isActive = false;
    private readonly int sparkleAnimation = Animator.StringToHash("Sparkle");
    private readonly int inactiveAnimation = Animator.StringToHash("Inactive");

    public override bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(tileCoords) || nonColliderMap.HasTile(tileCoords);
        bool below = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y - 1));
        return !self && below;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            collision.TryGetComponent<Entity>(out Entity entity);
            if (entity != null && entity.teleportable)
            {
                Deactivate();
                pair.TeleportTo(collision.transform);
                StartCoroutine(PortalCooldown()); 
            }
        }
    }

    public void Activate()
    {
        animator.CrossFade(sparkleAnimation, 0f);
        spriteRenderer.color = activeColor;
        isActive = true;
    }

    public void Deactivate()
    {
        animator.CrossFade(inactiveAnimation, 0f);
        spriteRenderer.color = inactiveColor;
        isActive = false;
    }

    // teleports the provided transform to this portals position
    public void TeleportTo(Transform _transform)
    {
        Deactivate();
        _transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        StartCoroutine(PortalCooldown());
    }

    private IEnumerator PortalCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        Activate();
    }
}
