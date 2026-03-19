using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireballProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * speed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
        } else if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().Die();
        }
    }
}
