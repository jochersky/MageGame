using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform parentTransform;
    
    public void SpawnObject(Transform spawnTransform)
    {
        GameObject go = Instantiate(prefab, parentTransform ? parentTransform : transform);
        go.transform.position = spawnTransform.position;
        go.transform.rotation = spawnTransform.rotation;
    }
}
