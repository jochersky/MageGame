using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float magneticForce = 100f;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            rb.AddForce((collision.transform.position - gameObject.transform.position) * magneticForce);
        }
    }
}
