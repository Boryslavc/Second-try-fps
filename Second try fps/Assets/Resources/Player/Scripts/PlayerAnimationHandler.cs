using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public static Action<float, Vector3, float> OnShootRecoil;

    [Header("References")]
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Transform _animationTarget;

    [Header("Sway")]
    [SerializeField] private float step = 0.1f;
    [SerializeField] private float maxStepDistance = 0.06f;
    private Vector3 swayPosition;

    [Header("Sway Rotation")]
    [SerializeField] private float rotationStep = 0.1f;
    [SerializeField] private float maxRotationStep = 0.06f;
    private Vector3 swayEulerRotation;

    [Header("Smoothing")]
    [SerializeField] private float smooth = 10f;
    [SerializeField] private float smoothRot = 12f;


    private InputHandler _inputHandler;

    private float _recoilSpeed = 10f;
    private Vector3 _recoilAmount = new Vector3(.1f, .1f, .1f);
    private float _recoilReturnSpeed = 2f;


    private Vector3 _recoilRotation;
    private Vector3 _recoilRotationCurrent;

    void Awake()
    {
        OnShootRecoil += OnShootRecoil_Recoil;
        _inputHandler = GetComponent<InputHandler>();
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
        Vector3 invertLook = _inputHandler.LookInput * -step;
        //Debug.Log(invertLook);
        invertLook.x = Mathf.Clamp(invertLook.x, -maxStepDistance, maxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxStepDistance, maxStepDistance);

        swayPosition = invertLook;
    }

    private void SwayRotation()
    {
        Vector3 invertLook = _inputHandler.LookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRotation = new Vector3(invertLook.y, invertLook.x, invertLook.x);

    }

    private void CompositePositionRotation()
    {
        _animationTarget.localPosition = Vector3.Lerp(_animationTarget.localPosition, swayPosition, Time.deltaTime * smooth);
        _animationTarget.localRotation = Quaternion.Slerp(_animationTarget.localRotation,
            Quaternion.Euler(swayEulerRotation), Time.deltaTime * smoothRot);
    }

    private void LateUpdate()
    {
        Sway();
        SwayRotation();
        CompositePositionRotation();
    }


}
