using UnityEngine;

public class Heart : Pickup
{
    public override void PickUpEffect()
    {
        Debug.Log("Heart collected");
    }
}
