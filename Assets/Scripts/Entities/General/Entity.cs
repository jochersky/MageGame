using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Properties")]
    [SerializeField] private bool freezable = true;

    protected bool frozen;
    private float _freezeDuration;
    private float _freezeTimer;
    
    private Coroutine _freezeRoutine;
    
    public void Freeze(float freezeDuration)
    {
        if (!freezable) return;
        
        _freezeDuration = freezeDuration;

        if (_freezeRoutine != null) return;
        _freezeRoutine = StartCoroutine(FreezeForDuration());
    }

    public IEnumerator FreezeForDuration()
    {
        frozen = true;
        _freezeTimer = 0f;
        while (_freezeTimer < _freezeDuration)
        {
            _freezeTimer += Time.deltaTime;
            yield return null;
        }
        frozen = false;
    }
}
