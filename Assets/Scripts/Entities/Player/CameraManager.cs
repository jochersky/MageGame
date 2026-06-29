using UnityEngine;

public class CameraManager : MonoBehaviour
{
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

    private void Start()
    {
        Keyframe k = yDownShiftCurve[0];
        _initialPosition = new Vector3(transform.localPosition.x, k.value, transform.localPosition.z);
        // k = yDownShiftCurve[yDownShiftCurve.length - 1];
        // _downPosition = new Vector3(transform.localPosition.x, k.value, transform.localPosition.z);
        _downEndPointTime = yDownShiftCurve[yDownShiftCurve.length - 1].time;
        
        _upEndPointTime = yUpShiftCurve[yUpShiftCurve.length - 1].time;
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
}
