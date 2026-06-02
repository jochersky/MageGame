using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PoisonPlant : MonoBehaviour
{
    [SerializeField] float interval = 1f;
    [SerializeField] PoisonBubble poisonBubble;
    [SerializeField] float spawnOffset = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnBubble), interval, interval);
    }

    // Update is called once per frame
    void SpawnBubble() 
    {
        Instantiate(poisonBubble, new Vector3(transform.position.x, transform.position.y + spawnOffset, transform.position.z), Quaternion.identity);
    }
}
