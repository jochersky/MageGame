using UnityEngine;

public class DestroyableAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private float _time = 0f;
    void Update()
    {
        if (_time > lifetime)
        {
            Destroy(gameObject);
        }
        _time += Time.deltaTime;
    }
}
