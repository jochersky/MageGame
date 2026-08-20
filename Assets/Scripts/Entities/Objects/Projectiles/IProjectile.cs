using UnityEditor.Search;
using UnityEngine;

public interface IProjectile 
{
    public void Initialize(LineOfSightSensor targetSensor, ProjectileManager projectileManager);
}
