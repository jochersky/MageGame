using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Dart : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 500f;
    public Vector3 direction;
    [SerializeField] TemporaryEffect hitWallEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Environment"))
        {
            Instantiate(hitWallEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        } 
    }
}
