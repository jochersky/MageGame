using UnityEngine;

public class Heart : Pickup
{
    [SerializeField] private int healthRestored = 1;
    
    public override void PickUpEffect()
    {
        GameManager.Instance.PlayerHealth.Heal(healthRestored);
    }
}
