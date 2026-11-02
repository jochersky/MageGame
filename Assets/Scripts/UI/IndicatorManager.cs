using System;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    // the objects that the indicator will point towards
    [SerializeField] private List<GameObject> targets;
    // indicator prefab to instaniate for each target
    [SerializeField] GameObject indicatorPrefab;
    // sprite size data necessary to ensure it is always visible on the edge of the screen
    private SpriteRenderer _indicatorSprite;
    private bool active = false;
    private float _spriteHeight;
    private float _spriteWidth;
    private Camera _camera;
    // map designates which indicator is associate with which target
    private Dictionary<GameObject, GameObject> _targetMap = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       MapGenerator mapGenerator = FindAnyObjectByType<MapGenerator>();
       if (mapGenerator != null)
        {
            mapGenerator.OnPlayerPlaced += OnPlayerPlaced;
        } else
        {
            OnPlayerPlaced(null);
        }
    }

    void OnPlayerPlaced(Player _player)
    {
        // obtain camera and indicator sprite data
        _camera = Camera.main;
        _indicatorSprite = indicatorPrefab.GetComponent<SpriteRenderer>();
        _spriteHeight = _indicatorSprite.bounds.extents.y;
        _spriteWidth = _indicatorSprite.bounds.extents.x;
    }

    public void InstantiateIndicators()
    {
        // instantiate indicators but mark them inactive for now
        foreach (GameObject target in targets)
        {
            GameObject indicator = Instantiate(indicatorPrefab);
            indicator.SetActive(false);
            _targetMap.Add(target, indicator);
        }

        active = true;
    }

    public void AddTarget(GameObject target)
    {
        targets.Add(target);
    }

    public void ClearTargets()
    {
        targets.Clear();
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            foreach (KeyValuePair<GameObject, GameObject> kvp in _targetMap)
            {
                GameObject target = kvp.Key;
                GameObject indicator = kvp.Value;
                UpdateTarget(target, indicator);
            } 
        }
        
    }

    private void UpdateTarget(GameObject target, GameObject indicator)
    {
        if (target == null)
        {
            Destroy(indicator);
            active = false;
            return;
        }
        Vector3 targetPos = _camera.WorldToViewportPoint(target.transform.position);
        bool offscreen = targetPos.x <= 0 || targetPos.x >= 1 || targetPos.y <= 0 || targetPos.y >= 1;
        if (offscreen)
        {
            indicator.SetActive(true);
            // clamp position to edge of screen
            Vector3 viewportSize = _camera.WorldToViewportPoint(new Vector3(_spriteWidth, _spriteHeight, 0)) - _camera.WorldToViewportPoint(Vector3.zero);
            targetPos.x = Mathf.Clamp(targetPos.x, viewportSize.x, 1f - viewportSize.x);
            targetPos.y = Mathf.Clamp(targetPos.y, viewportSize.y, 1f - viewportSize.y);

            // set its position
            Vector3 worldPosition = _camera.ViewportToWorldPoint(targetPos);
            targetPos.z = 0;
            indicator.transform.position = worldPosition;

            // rotate indicator to point to target
            Vector3 directionVector = target.transform.position - indicator.transform.position;
            float angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        } else
        {
            indicator.SetActive(false);
        }
    }
}
