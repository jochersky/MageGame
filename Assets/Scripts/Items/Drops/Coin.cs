using UnityEngine;

public class Coin : Pickup
{
    public override void PickUpEffect()
    {
        Debug.Log("Coin Collected");
    }
} 