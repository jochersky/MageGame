using UnityEngine;
using UnityEngine.Events;

public class FlipTransform : MonoBehaviour
{
    public void Flip(float sign)
    {
        transform.localScale = new Vector3(sign, transform.localScale.y, transform.localScale.z);
    }
}
