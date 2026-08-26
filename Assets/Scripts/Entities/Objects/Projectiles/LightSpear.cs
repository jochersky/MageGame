using System.Collections;
using UnityEngine;

public class LightSpear : MonoBehaviour, IProjectile
{
    [Header("References")]
    [SerializeField] private ParticleSystem hitParticles;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [Header("Properties")]
    [SerializeField] private float timeUntilPursuit = 5;
    [SerializeField] private float speed = 5;
    [SerializeField] private float acceleration;
    
    public LineOfSightSensor _targetSensor;
    private ProjectileManager _projectileManager;
    public Transform _target;
    private bool _removedFromManager;
    private CountdownTimer _timer;
    private bool _targetAcquired = false;

    public void Initialize(LineOfSightSensor targetSensor, ProjectileManager projectileManager)
    {
        _targetSensor = targetSensor;
        _projectileManager = projectileManager;
    }
    
    private void Start()
    {
        _timer = new CountdownTimer(timeUntilPursuit);
        _timer.OnTimerStop += CheckForTarget;
    }
    
    void Update()
    {
        if (!_targetSensor) return;
        
        if (!_target)
        {
            _target = _targetSensor.GetNextTarget()?.transform;
            _timer.Start();
        }
        else if (_targetAcquired)
        {
            if (!_removedFromManager)
            {
                _projectileManager.RemoveProjectile(this.gameObject);
                _removedFromManager = true;
            }
            
            // update rotation towards the target
            Vector3 dir = _target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            // update position to track
            transform.position = Vector3.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);
        }
        
        _timer.Tick(Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_targetAcquired && collision.CompareTag("Environment")) || collision.CompareTag("Enemy"))
        {
            _projectileManager.RemoveProjectile(gameObject);
            StartCoroutine(DestroySequence());
        }
    }
    

    private void CheckForTarget()
    {
        Transform temp = _targetSensor.GetNextTarget()?.transform;
        if (temp && temp.gameObject == _target.gameObject)
        {
            // pursue the confirmed target
            _targetAcquired = true;
        }
        if (temp)
        {
            // new target found, rerun timer to confirm target
            _timer.Start();
        }
        // target gets rechecked in the update loop
        _target = temp;
    }

    private IEnumerator DestroySequence()
    {
        spriteRenderer.enabled = false;
        hitParticles.Play();

        yield return new WaitForSeconds(hitParticles.main.startLifetime.constantMax);
        
        Destroy(gameObject);
    }
}
