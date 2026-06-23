using UnityEngine;

public class Player : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.Player = this;
    }
    
    public void Save(ref PlayerSaveData data)
    {
        data.position = transform.position;
    }

    public void Load(ref PlayerSaveData data)
    {
        transform.position = data.position;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 position;
}