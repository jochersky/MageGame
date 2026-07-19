using System;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class Chain : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SplineContainer spline;
    [SerializeField] private Transform[] links;
    [Header("Properties")]
    [SerializeField] private float distance = 0.25f;
    [SerializeField] private float startingProgress = 0.25f;
    
    private float _t;
    private float _progress;
    
    public float Progress => _progress;

    private void Start()
    {
        _t = startingProgress;
    }
    
    public void UpdateChain(float progressSpeed, float followSpeed)
    {
        _progress = _t % 1;
        
        for (int i = 0; i < links.Length; i++)
        {
            Transform link = links[i];
            float linkProgress = Math.Max(0, _progress - (distance * i)) % 1;;
            
            link.position = Vector3.MoveTowards(link.position, spline.EvaluatePosition(linkProgress), followSpeed * Time.deltaTime);
            
            Vector3 tangent = spline.EvaluateTangent(linkProgress);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            link.rotation = Quaternion.Euler(0, 0, angle);
        }

        _t += Time.fixedDeltaTime * progressSpeed;
    }

    public void SyncChain()
    {
        foreach (Transform link in links)
        {
            link.position = spline.EvaluatePosition(0.25f);
            Vector3 tangent = spline.EvaluateTangent(0.25f);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
            link.rotation = Quaternion.Euler(0, 0, angle);
            _progress = 0.25f;
            _t = 0.25f;
        }
    }
}
