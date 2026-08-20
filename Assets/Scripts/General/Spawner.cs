using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform parentTransform;
    [Header("Properties")]
    [SerializeField] int spawnCount;
    [SerializeField] private float spawnRadius;
    [SerializeField] private bool randomizeRotation;
    [SerializeField] private float rotationRange;
    
    public void SpawnObject(Transform spawnTransform)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject go = Instantiate(prefab, parentTransform ? parentTransform : transform);
            
            Vector3 randomPosInCircle = Random.insideUnitCircle * spawnRadius;
            go.transform.position = spawnTransform.position + randomPosInCircle;

            if (randomizeRotation)
            {
                Vector3 rot = transform.rotation.eulerAngles;
                rot.z += Random.Range(-rotationRange, rotationRange);
                go.transform.rotation = Quaternion.Euler(rot);
            }
            else
            {
                go.transform.rotation = spawnTransform.rotation;
            }
        }
    }
}
