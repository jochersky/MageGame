using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public delegate void Died();
    public event Died OnDied;
    public void Die()
    {
        print("Updates");
        OnDied?.Invoke();
        //Destroy(gameObject);
    }
}
