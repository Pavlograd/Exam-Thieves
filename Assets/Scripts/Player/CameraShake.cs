using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform _cameraTransform;
    private Vector3 _originalPosCam;
    private Vector3 _originalPosCamShake;
    [SerializeField] private float shakeFrequency;
    private bool _isShake = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = gameObject.transform;
        _originalPosCam = _cameraTransform.localPosition;
        
    }

    public void StartShake(Transform headGardTransform)
    {
        _originalPosCam = _cameraTransform.localPosition;
        _originalPosCamShake = _cameraTransform.position;
        _cameraTransform.LookAt(headGardTransform);
        //_cameraTransform.rotation = Quaternion.Euler(new Vector3(0, _cameraTransform.eulerAngles.y, _cameraTransform.eulerAngles.z));
        _isShake = true;
    }

    private void CamereShake()
    {
        _cameraTransform.position = _originalPosCamShake + Random.insideUnitSphere * shakeFrequency;
    }

    public void StopShake()
    {
        _cameraTransform.localPosition = _originalPosCam;
        //_cameraTransform.rotation = new Quaternion(0, 0, 0, 0);
        _isShake = false;
    }

    public void SetCameraFront()
    {
        _cameraTransform.rotation = new Quaternion(0, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isShake)
            CamereShake();
    }
}
