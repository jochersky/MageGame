using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Portal : Trap
{
    // Manually assign which portal this portal will be paired with
    public Portal pair;
    [SerializeField] float cooldown;
    private bool isActive = false;

    public override bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(tileCoords) || nonColliderMap.HasTile(tileCoords);
        return !self;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            print(collision);
            isActive = false;
            pair.TeleportTo(collision.transform);
            StartCoroutine(PortalCooldown());
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    // teleports the provided transform to this portals position
    public void TeleportTo(Transform _transform)
    {
        _transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        StartCoroutine(PortalCooldown());
    }

    private IEnumerator PortalCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        isActive = true;
    }
}
