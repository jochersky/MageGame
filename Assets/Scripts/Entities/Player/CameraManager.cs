using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Shake Properties")]
    [SerializeField] private Health health;
    [Header("Shift Properties")]
    [SerializeField] private float yShiftSpeed = 2f;
    [SerializeField] private AnimationCurve yDownShiftCurve;
    [SerializeField] private AnimationCurve yUpShiftCurve;

    private Vector3 _initialPosition;
    private bool _moveToDownPosition;
    private bool _moveToUpPosition;
    private float _downEndPointTime;
    private float _upEndPointTime;
    private float _curveTime;

    private bool _justShiftedDown;
    private bool _justShiftedUp;

    private float _shakeTimer;
    private Coroutine _shakeRoutine;

    private void Start()
    {
        Keyframe k = yDownShiftCurve[0];
        _initialPosition = new Vector3(transform.localPosition.x, k.value, transform.localPosition.z);
        // k = yDownShiftCurve[yDownShiftCurve.length - 1];
        // _downPosition = new Vector3(transform.localPosition.x, k.value, transform.localPosition.z);
        _downEndPointTime = yDownShiftCurve[yDownShiftCurve.length - 1].time;
        
        _upEndPointTime = yUpShiftCurve[yUpShiftCurve.length - 1].time;

        health.OnDamageTaken += ShakeCameraFromDamage;
    }

    private void Update()
    {
        if (_moveToDownPosition)
        {
            _curveTime = Mathf.Min(_curveTime + Time.deltaTime * yShiftSpeed, _downEndPointTime);
            transform.localPosition = new Vector3(transform.localPosition.x, yDownShiftCurve.Evaluate(_curveTime), transform.localPosition.z);
            _justShiftedDown = true;
        }
        else if (!_moveToDownPosition && _justShiftedDown)
        {
            _curveTime = Mathf.Max(_curveTime - Time.deltaTime * yShiftSpeed, 0);
            transform.localPosition = new Vector3(transform.localPosition.x, yDownShiftCurve.Evaluate(_curveTime), transform.localPosition.z);
            if (_curveTime <= 0) _justShiftedDown = false;
        }
        
        if (_moveToUpPosition)
        {
            _curveTime = Mathf.Min(_curveTime + Time.deltaTime * yShiftSpeed, _upEndPointTime);
            transform.localPosition = new Vector3(transform.localPosition.x, yUpShiftCurve.Evaluate(_curveTime), transform.localPosition.z);
            _justShiftedUp = true;
        }
        else if (!_moveToUpPosition && _justShiftedUp)
        {
            _curveTime = Mathf.Max(_curveTime - Time.deltaTime * yShiftSpeed, 0);
            transform.localPosition = new Vector3(transform.localPosition.x, yUpShiftCurve.Evaluate(_curveTime), transform.localPosition.z);
            if (_curveTime <= 0) _justShiftedUp = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Explosion>(out Explosion e))
        {
            ShakeCamera(e.CameraShakeProperties);
        }
    }

    public void ShiftCameraDown()
    {
        transform.localPosition = _initialPosition;
        _curveTime = 0;
        _moveToDownPosition = true;
    }
    
    public void ShiftCameraUp() 
    {
        transform.localPosition = _initialPosition;
        _curveTime = 0;
        _moveToUpPosition = true;
    }

    public void ReturnCameraToOriginalPosition()
    {
        _moveToDownPosition = false;
        _moveToUpPosition = false;
    }

    private void ShakeCamera(CameraShakeProperties shakeProperties)
    {
        // transform.localPosition = Random.insideUnitCircle * 0.25f;
        if (_shakeRoutine != null) StopAllCoroutines();
        
        _shakeRoutine = StartCoroutine(Shake(shakeProperties));
    }

    private void ShakeCameraFromDamage(DamageProperties damageProperties)
    {
        ShakeCamera(damageProperties.cameraShakeProperties);
    }
    
    private IEnumerator Shake(CameraShakeProperties shakeProperties)
    {
        _shakeTimer = 0;
        while (_shakeTimer <= shakeProperties.duration)
        {
            float shakeAmtAdjusted = shakeProperties.amount * (1 - (_shakeTimer / shakeProperties.duration));
            transform.localPosition = new Vector2(_initialPosition.x, _initialPosition.y) + (shakeAmtAdjusted * Random.insideUnitCircle);
            
            _shakeTimer += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = _initialPosition;
    }
}

public struct CameraShakeProperties
{
    public float amount;
    public float duration;
}
