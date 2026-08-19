using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Chain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private List<GameObject> links;
    [SerializeField] private ChainLinkLayerStrategy chainLinkLayer;
    [Header("Properties")]
    [SerializeField] private bool equidistantPositioning;
    [SerializeField] private float distance = 0.25f;
    [SerializeField] private float startingProgress = 0.25f;
    [SerializeField] private int frontLayer;
    [SerializeField] private int backLayer;
    // goToFrontPercentage < goToBackPercentage
    [SerializeField] private float goToFrontPercentage;
    [SerializeField] private float goToBackPercentage;
    
    private float _t;
    private float _progress;
    
    public float Progress => _progress;
    public List<GameObject> Links => links;
    public float Distance { get => distance; set => distance = value; }

    private void Start()
    {
        _t = startingProgress;

        goToFrontPercentage /= 100;
        goToBackPercentage /= 100;
    }
    
    public void UpdateChain(float progressSpeed, float followSpeed)
    {
        _progress = _t % 1;
        
        for (int i = 0; i < links.Count; i++)
        {
            if (!links[i]) continue; 
            
            Transform link = links[i].transform;
            
            float linkProgress = 0;
            if (equidistantPositioning)
            {
                float distFactor = i != 0 ? 1f / links.Count : 0f;
                linkProgress = _progress - distFactor * i;
            }
            else 
                linkProgress = _progress - i * distance;
            if (linkProgress < 0) 
                linkProgress += 1;
            
            link.position = Vector3.MoveTowards(link.position, spline.EvaluatePosition(linkProgress), followSpeed * Time.deltaTime);
            
            Vector3 tangent = spline.EvaluateTangent(linkProgress);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            link.rotation = Quaternion.Euler(0, 0, angle);

            if (frontLayer == 0 && backLayer == 0) continue;
            
            if (link.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                int currLayer = spriteRenderer.sortingOrder;
                if (currLayer != frontLayer && (linkProgress >= goToFrontPercentage && linkProgress < goToBackPercentage))
                {
                    spriteRenderer.sortingOrder = frontLayer;
                }
                else if (currLayer != backLayer && (linkProgress >= goToBackPercentage || linkProgress < goToFrontPercentage))
                {
                    spriteRenderer.sortingOrder = backLayer;
                }
            }
        }

        _t += Time.fixedDeltaTime * progressSpeed;
    }

    public void SyncChain()
    {
        foreach (GameObject go in links)
        {
            Transform link = go.transform;
            link.position = spline.EvaluatePosition(0.25f);
            Vector3 tangent = spline.EvaluateTangent(0.25f);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            link.rotation = Quaternion.Euler(0, 0, angle);
            _progress = 0.25f;
            _t = 0.25f;
        }
    }
}
