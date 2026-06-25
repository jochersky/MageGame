using UnityEngine;

public class Coin : Pickup
{
    public override void PickUpEffect()
    {
        InventoryManager.Instance.UpdateMoney(1);
    }
} 