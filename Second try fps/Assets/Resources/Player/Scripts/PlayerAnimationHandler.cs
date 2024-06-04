using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public static Action<float, Vector3, float> OnShootRecoil;

    [Header("References")]
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _animationTarget;

    [Header("Sway")]
    [SerializeField] private float _step = 0.1f;
    [SerializeField] private float _maxStepDistance = 0.06f;
    private Vector3 _swayPosition;

    [Header("Sway Rotation")]
    [SerializeField] private float _rotationStep = 0.1f;
    [SerializeField] private float _maxRotationStep = 0.06f;
    private Vector3 _swayEulerRotation;

    [Header("Bobbing")]
    [SerializeField] private float _speedCurve;
    private float _curveSin
    {
        get => MathF.Sin(_speedCurve);
    }

    private float _curveCos
    {
        get => MathF.Cos(_speedCurve);
    }

    [SerializeField] private Vector3 _travelLimit = Vector3.one * 0.025f;
    [SerializeField] private Vector3 _boblLimit = Vector3.one * 0.01f;
    [SerializeField] private Vector3 _bobRotationMultiplier;
    private Vector3 _bobPosition;
    private Vector3 _bobEulerRotation;


    [Header("Smoothing")]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float smoothRot = 12f;


    private InputHandler _inputHandler;
    private CharacterController _characterController;

    private float _recoilSpeed = 10f;
    private Vector3 _recoilAmount = new Vector3(.1f, .1f, .1f);
    private float _recoilReturnSpeed = 2f;

    private Vector3 _recoilRotation;
    private Vector3 _recoilRotationCurrent;

    void Awake()
    {
        OnShootRecoil += OnShootRecoil_Recoil;
        _inputHandler = GetComponent<InputHandler>();
        _characterController = GetComponent<CharacterController>();
    }

    void OnDisable()
    {
        OnShootRecoil -= OnShootRecoil_Recoil;
    }

    void Update()
    {
        _recoilRotation = Vector3.Lerp(_recoilRotation, Vector3.zero, _recoilReturnSpeed * Time.deltaTime);
        _recoilRotationCurrent = Vector3.Slerp(_recoilRotationCurrent, _recoilRotation, _recoilSpeed * Time.deltaTime);
        _playerCamera.transform.localEulerAngles = _recoilRotationCurrent;
    }

    private void OnShootRecoil_Recoil(float recoilSpeed, Vector3 recoilAmount, float recoilReturnSpeed)
    {
        _recoilSpeed = recoilSpeed;
        _recoilAmount = recoilAmount;
        _recoilReturnSpeed = recoilReturnSpeed;

        _recoilRotation += _recoilAmount;
    }

    private void Sway()
    {
        Vector3 invertLook = _inputHandler.LookInput * -_step;
        invertLook.x = Mathf.Clamp(invertLook.x, -_maxStepDistance, _maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -_maxStepDistance, _maxStepDistance);

        _swayPosition = invertLook;
    }

    private void SwayRotation()
    {
        Vector3 invertLook = _inputHandler.LookInput * -_rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -_maxRotationStep, _maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -_maxRotationStep, _maxRotationStep);

        _swayEulerRotation = new Vector3(invertLook.y, invertLook.x, invertLook.x);

    }

    private void BobOffset()
    {
        _speedCurve += Time.deltaTime * (_characterController.isGrounded ? _characterController.velocity.magnitude : 1f) + 0.001f;

        _bobPosition.x = (_curveCos * _boblLimit.x * (_characterController.isGrounded ? 1 : 0)) - (_inputHandler.MoveInput.x * _travelLimit.x);
        _bobPosition.y = (_curveSin * _boblLimit.y) - (_inputHandler.MoveInput.y * _travelLimit.y);
        _bobPosition.z = - (_inputHandler.MoveInput.y * _travelLimit.z);
    }

    private void BobRotation()
    {
        _bobEulerRotation.x = (_inputHandler.MoveInput != Vector2.zero ? _bobRotationMultiplier.x * (MathF.Sin(2 * _speedCurve)) :
                                                                         _bobRotationMultiplier.x * (MathF.Sin(2 * _speedCurve) / 2)); //pitch

        _bobEulerRotation.y = (_inputHandler.MoveInput != Vector2.zero ? _bobRotationMultiplier.y * _curveCos : 0); //yaw

        _bobEulerRotation.y = (_inputHandler.MoveInput != Vector2.zero ? _bobRotationMultiplier.z * _curveCos * _inputHandler.MoveInput.x : 0); //roll
        
    }

    private void CompositePositionRotation() {
        _animationTarget.localPosition = Vector3.Lerp(_animationTarget.localPosition, _swayPosition + _bobPosition, Time.deltaTime * smooth);
        _animationTarget.localRotation = Quaternion.Slerp(_animationTarget.localRotation,
            Quaternion.Euler(_swayEulerRotation) * Quaternion.Euler(_bobEulerRotation), Time.deltaTime * smoothRot);
    }

    private void LateUpdate()
    {
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();
        CompositePositionRotation();
    }


}
