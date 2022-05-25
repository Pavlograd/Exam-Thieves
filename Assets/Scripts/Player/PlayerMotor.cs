using System.Collections;
using SCPE;
using StaticClassSettingsGame;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private float speedCrouchCamera = 1f;
    [SerializeField] private float valueVignetteNormale = 0.3f;
    [SerializeField] private float valueVignetteCrounch = 0.5f;
    [SerializeField] private Vector3 cameraHight;
    [SerializeField] private Vector3 cameraCrounch;
    [SerializeField] private Vector3 centerCharacterController;
    [SerializeField] private Vector3 centerCharacterControllerCrouch;
    [SerializeField] private float heightCharacterControllerCrouch;
    [SerializeField] private float heightCharacterController;
    [SerializeField] private float smoothCameraTime;
    [SerializeField] private float motionBlur;
    [SerializeField] private float motionBlurSpeed;
    [SerializeField] private float speedLineSpeed;

    private bool _isCrouching;
    private bool _duringCrounchAnimnation;

    private bool _shouldCrounch = false;
    private bool _forceCrounch = false;
    private CharacterController _controller;

    [SerializeField] private Camera cam;
    [SerializeField] private float cameraRotationLimit = 85f;
    private Vector3 _velocity;
    private Vector3 _rotation;
    private float _cameraRotationX = 0f;
    private float _currentCameraRotationX = 0f;
    private Animator _animator;
    private Volume _volume;
    public Vignette _vignette;
    private MotionBlur _motionBlur;
    private SpeedLines _speedLines;
    private bool _isRun;
    [SerializeField] private float durationVignette;
    private Vector3 _verticalAngularVelocity;
    private Vector3 _horizontalAngularVelocity;
    private bool _isCatched;
    private Player _player;

    private float crounchRate = 0.2f;
    private float nextCrounch = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        _controller = gameObject.GetComponent<CharacterController>();
        _player = gameObject.GetComponent<Player>();
        _volume = GameObject.FindWithTag("PostProcessing").GetComponent<Volume>();
        _volume.profile.TryGet(out _vignette);
        _volume.profile.TryGet(out _motionBlur);
        _volume.profile.TryGet(out _speedLines);
    }

    public bool getIsCrounching()
    {
        return _isCrouching;
    }

    public void Move(Vector3 _velocity, Vector3 _rotation, float _cameraRotationX, bool _isRun, bool _shouldCrounch, bool _isCatched)
    {
        this._velocity = _velocity;
        this._rotation = _rotation;
        this._cameraRotationX = _cameraRotationX;
        this._isRun = _isRun;
        this._shouldCrounch = _shouldCrounch;
        this._isCatched = _isCatched;
    }

    void FixedUpdate()
    {
        if (!_player.isDeath && !PauseMenu.isOn)
        {
            PerformMovement();
            PerformRotation();
            PerformCrounch();
        }
        cam.transform.parent.localPosition = _isCrouching 
            ? Vector3.Lerp(cam.transform.parent.localPosition, cameraCrounch, speedCrouchCamera * Time.fixedDeltaTime)
            : Vector3.Lerp(cam.transform.parent.localPosition, cameraHight, speedCrouchCamera *Time.fixedDeltaTime);
        _vignette.intensity.Override( _isCrouching
            ? Mathf.Lerp(_vignette.intensity.value, valueVignetteCrounch, speedCrouchCamera * Time.fixedDeltaTime)
            : Mathf.Lerp(_vignette.intensity.value, valueVignetteNormale, speedCrouchCamera * Time.fixedDeltaTime));
        _speedLines.intensity.Override( _isRun
            ? Mathf.Lerp(_speedLines.intensity.value, speedLineSpeed, speedCrouchCamera * Time.fixedDeltaTime)
            : Mathf.Lerp(_speedLines.intensity.value, 0, speedCrouchCamera * Time.fixedDeltaTime));
    }

    private void PerformCrounch()
    {
        if (!_shouldCrounch) return;
        if (Time.fixedTime >= nextCrounch)
        {
            Debug.Log("Crounch");
            _isCrouching = !_isCrouching;
            _controller.center = _isCrouching ? centerCharacterControllerCrouch : centerCharacterController;
            _controller.height = _isCrouching ? heightCharacterControllerCrouch : heightCharacterController;
            _animator.SetBool("Crounch", _isCrouching);
            Debug.Log("Valeur de Crounch dans l'animator " + _animator.GetBool("Crounch"));
            nextCrounch = Time.fixedTime + crounchRate;
        }
    }
    

    private void PerformMovement()
    {

        if (_velocity != Vector3.zero)
        {
            if (_isCrouching)
            {
                _velocity = _velocity / 2;
            }

            _controller.Move(_velocity * Time.fixedDeltaTime);
        }

        //_rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    private void PerformRotation()
    {
        if (_isCatched) return;
        _rotation *= Time.fixedDeltaTime;
        _cameraRotationX *= Time.fixedDeltaTime;
        //_rb.MoveRotation(_rb.rotation * Quaternion.Euler(_rotation));
        transform.Rotate(Vector3.up * _rotation.y);
        /*Vector3.SmoothDamp(transform.rotation.eulerAngles,
            Vector3.up * _rotation.y, ref _horizontalAngularVelocity, smoothCameraTime * Time.fixedDeltaTime);*/
        //On calcule la rotation de la camera et avec Mathf Clamp on limit l'angle par rapport à une variable.
        //Le -= sigifie vue normal et si on veux vue inverser il faut +=
        if (PlayerSettings.inverseAxis)
            _currentCameraRotationX += _cameraRotationX;
        else
            _currentCameraRotationX -= _cameraRotationX;
        _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
     /*  if (test)
       {
           cam.transform.localEulerAngles = Vector3.SmoothDamp(cam.transform.parent.localEulerAngles,
               new Vector3(_currentCameraRotationX, 0f, 0f), ref _verticalAngularVelocity,
               smoothCameraTime * Time.fixedDeltaTime);
       }
        else
        {
            cam.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
        }*/
       cam.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
    }
}