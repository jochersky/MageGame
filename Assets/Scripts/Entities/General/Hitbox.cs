using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damageAmt;

    [Header("Hitbox Colliders")] 
    [SerializeField] private Collider2D[] colliders;

    public void Disable()
    {
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
}
