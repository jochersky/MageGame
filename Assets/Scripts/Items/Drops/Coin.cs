using UnityEngine;

public class Coin : Pickup
{
    public override void PickUpEffect()
    {
        // Debug.Log("Coin Collected");
        MoneyCounter mc = FindFirstObjectByType<MoneyCounter>();
        mc.AddMoney(1);
    }
} 