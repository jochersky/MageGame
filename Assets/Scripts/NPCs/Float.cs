using UnityEngine;

public class Float : MonoBehaviour
{
    // determines how the range of movement
    [SerializeField] float amplitude = 0.15f;
    // determines how the speed of movement
    [SerializeField] float speed = 2f;
    private Vector3 startingPosition;
    void Start()
    {
        startingPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        // simple harmonic motion formula: y = A * sin(wt)
        float y = startingPosition.y + amplitude * Mathf.Sin(speed * Time.time);
        transform.position = new Vector3(startingPosition.x, y, startingPosition.z);
    }
}
