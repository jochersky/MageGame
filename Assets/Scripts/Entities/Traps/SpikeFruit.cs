using UnityEngine;
using UnityEngine.Tilemaps;

public class SpikeFruit : Trap
{
    [SerializeField] Spike spike;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TemporaryEffect burstEffect;
    private bool falling = false;
    public override bool CheckIfValidPosition(TileBase currTile, Vector3Int tileCoords, Tilemap colliderMap, Tilemap nonColliderMap)
    {
        bool self = colliderMap.HasTile(tileCoords) || nonColliderMap.HasTile(tileCoords);
        bool above = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y + 1));
        bool below = colliderMap.HasTile(new Vector3Int(tileCoords.x, tileCoords.y - 1));
        return !self && above && !below;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (falling && collision.CompareTag("Environment"))
        {
            Burst();
        } else if (!falling && (!collision.isTrigger || collision.CompareTag("Hitbox")))
        {
            print("triggered by " + collision.name);
            Fall();   
        }
    }

    void Burst()
    {
        Instantiate(burstEffect, transform.position, Quaternion.identity);
        // send spikes left, up, and right   
        //spike.transform.SetPositionAndRotation(spike.transform.position, Quaternion.Euler(0, 0, 0));
        spike.directionFired = -transform.right;
        Spike instance = Instantiate(spike, transform.position, Quaternion.identity);
        instance.transform.SetPositionAndRotation(instance.transform.position, Quaternion.Euler(0, 0, 0));
        //spike.transform.SetPositionAndRotation(spike.transform.position, Quaternion.Euler(0, 0, 270));
        spike.directionFired = transform.up;
        instance = Instantiate(spike, transform.position, Quaternion.identity);
        instance.transform.SetPositionAndRotation(instance.transform.position, Quaternion.Euler(0, 0, 270));
        //spike.transform.SetPositionAndRotation(spike.transform.position, Quaternion.Euler(0, 0, 180));
        spike.directionFired = transform.right;
        instance = Instantiate(spike, transform.position, Quaternion.identity);
        instance.transform.SetPositionAndRotation(instance.transform.position, Quaternion.Euler(0, 0, 180));
        Destroy(gameObject);
    }

    void Fall()
    {
        rb.WakeUp();
        falling = true;
    }
}
