using UnityEngine;

public class Heart : Pickup
{
    [SerializeField] private int healthRestored = 1;
    
    public override void PickUpEffect()
    {
        Health health = FindFirstObjectByType<Player>().Health;
        health.Heal(healthRestored);
    }
}
