using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
    public static Action<float, Vector3, float> OnShootRecoil;

    [Header("References")]
    [SerializeField] private Transform _playerCamera;

    private float _recoilSpeed = 10f;
    private Vector3 _recoilAmount = new Vector3(.1f, .1f, .1f);
    private float _recoilReturnSpeed = 2f;


    private Vector3 _recoilRotation;
    private Vector3 _recoilRotationCurrent;

    void Awake()
    {
        OnShootRecoil += OnShootRecoil_Recoil;
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


}
