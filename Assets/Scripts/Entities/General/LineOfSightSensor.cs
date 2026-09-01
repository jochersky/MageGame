using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

/*
 * Every time an enemy enters and exits the radius we update their existence in the target list.
 * When a target is sampled by a caller, check to see if the targets are in the line of sight
 * but also their distance from the origin.
 * Return closest target that is in line of sight.
 */

public class LineOfSightSensor : MonoBehaviour
{
    [SerializeField] private string[] tagsToSense;
    [SerializeField] private string layerMaskToRaycast;
    [SerializeField] private Transform LineOfSightTransform;
    [SerializeField] private float maxLineOfSightDistance = 5;
    [SerializeField] private int maxTargetsToConsider = 5;
    [SerializeField] private bool debug;
    private LayerMask _mask;
    
    private List<Health> _nextTargets;
    
    private void Awake()
    {
        _nextTargets = new List<Health>();
    }

    private void Start()
    {
        if (!LineOfSightTransform) LineOfSightTransform = transform;
        
        _mask = LayerMask.GetMask(layerMaskToRaycast, "Environment");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!tagsToSense.Contains(other.tag)) return;

        if (other.TryGetComponent<Health>(out Health health))
        {
            if (health.CurrentHealth <= 0 || _nextTargets.Count > maxTargetsToConsider) return;
            
            _nextTargets.Add(health);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!tagsToSense.Contains(other.tag)) return;
        
        if (other.TryGetComponent<Health>(out Health health))
        {
            _nextTargets.Remove(health);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!tagsToSense.Contains(other.tag)) return;
        
        if (other.TryGetComponent<Health>(out Health health))
        {
            if (_nextTargets.Count > maxTargetsToConsider || _nextTargets.Contains(health)) return;

            _nextTargets.Add(health);
        }
    }

    public GameObject GetNextTarget()
    {
        List<Health> sortedObjects = new List<Health>();
        sortedObjects.AddRange(_nextTargets);
        sortedObjects.Sort((a, b) => Vector2.Distance(a.transform.position, transform.position).CompareTo(Vector2.Distance(b.transform.position, transform.position)));

        foreach (Health tar in sortedObjects)
        {
            if (tar.CurrentHealth <= 0) continue;
            Vector2 dir = (tar.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(LineOfSightTransform.position, dir, maxLineOfSightDistance, _mask);
            if (debug) Debug.DrawRay(LineOfSightTransform.position, dir * maxLineOfSightDistance, Color.red, 0.5f);
            if (hit && tagsToSense.Contains(hit.collider.tag))
            {
                return tar.gameObject;
            }
        }
        
        return null;
    }
}
