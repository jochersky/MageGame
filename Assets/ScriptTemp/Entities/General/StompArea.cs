using UnityEngine;
using UnityEngine.Events;

public class StompArea : MonoBehaviour
{
    public UnityEvent onAreaEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onAreaEntered?.Invoke();
    }
}
