using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Chain projectileChain;
    [Header("Properties")]
    [SerializeField] private int maxProjectiles = 5;
    [SerializeField] private float progressSpeed;
    [SerializeField] private float followSpeed;
    [SerializeField] private float distanceAdjust;
    
    public bool MaxProjectiles => projectileChain.Links.Count >= maxProjectiles;
    
    private void FixedUpdate()
    {
        projectileChain.UpdateChain(progressSpeed, followSpeed);
    }

    public void AddProjectile(GameObject projectile)
    {
        projectileChain.Links.Add(projectile);
        projectileChain.Distance -= distanceAdjust;
    }

    public void RemoveProjectile(GameObject projectile)
    {
        projectileChain.Links.Remove(projectile);
        projectile.transform.parent = null;
        projectileChain.Distance -= distanceAdjust;
    }
}
