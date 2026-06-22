using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private float yShiftSpeed = 2f;
    [SerializeField] private AnimationCurve yShiftCurve;

    private Vector3 _initialPosition;
    private Vector3 _downPosition;
    private bool _moveToDownPosition;
    private float _endPointTime;
    private float _curveTime;

    private void Start()
    {
        Keyframe k = yShiftCurve[0];
        _initialPosition = new Vector3(transform.localPosition.x, k.value, transform.localPosition.z);
        k = yShiftCurve[yShiftCurve.length - 1];
        _downPosition = new Vector3(transform.localPosition.x, k.value, transform.localPosition.z);
        _endPointTime = yShiftCurve[yShiftCurve.length - 1].time;
    }

    private void Update()
    {
        if (_moveToDownPosition) _curveTime = Mathf.Min(_curveTime + Time.deltaTime * yShiftSpeed, _endPointTime);
        else _curveTime = Mathf.Max(_curveTime - Time.deltaTime * yShiftSpeed, 0);
        
        transform.localPosition = new Vector3(transform.localPosition.x, yShiftCurve.Evaluate(_curveTime), transform.localPosition.z);
    }

    public void ShiftCameraDown()
    {
        _moveToDownPosition = true;
    }

    public void ReturnCameraToOriginalPosition()
    {
        _moveToDownPosition = false;
    }
}
