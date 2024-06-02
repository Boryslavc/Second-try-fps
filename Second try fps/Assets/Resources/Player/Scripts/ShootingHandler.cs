using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootingHandler : MonoBehaviour
{
    [Header("All guns available for player")]
    [SerializeField] private List<Gun> _gunsPrefabs;
    [SerializeField] private Transform _gunOffset; //positional offset of a gun

    private InputHandler inputHandler;
    private Camera camera;

    private List<Gun> gunList = new List<Gun>();
    private int currentGunIndex;


    public Type GetCurrentGunType()
    {
        return gunList[currentGunIndex].GetType();
    }

    private void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        camera = GetComponentInChildren<Camera>();

        InstantiateGuns();
    }

    private void InstantiateGuns()
    {
        foreach (Gun gun in _gunsPrefabs)
        {
            var gunModel = Instantiate(gun, _gunOffset);
            gunModel.gameObject.SetActive(false);
            gunModel.SetRayCastOrigin(camera.transform);
            gunList.Add(gunModel);
        }

        currentGunIndex = 0;
        gunList[currentGunIndex].gameObject.SetActive(true);
    }


    private void Update()
    {
        if (inputHandler.ChangeWeaponInt != 0)
            ChangeWeapon();

        HandleShooting();
    }

    private void ChangeWeapon()
    {
        gunList[currentGunIndex].gameObject.SetActive(false);

        if (IndexOutOfRange())
            ClampIndex();
        else
            currentGunIndex += inputHandler.ChangeWeaponInt;

        gunList[currentGunIndex].gameObject.SetActive(true);
    }
    private bool IndexOutOfRange()
    {
        return (currentGunIndex == 0 && inputHandler.ChangeWeaponInt == -1)
            || (currentGunIndex == gunList.Count - 1 &&  inputHandler.ChangeWeaponInt == 1);
    }
    private void ClampIndex()
    {
        if(currentGunIndex == 0)
            currentGunIndex = gunList.Count - 1;
        else
            currentGunIndex = 0;
    }

    private void HandleShooting()
    {
        gunList[currentGunIndex].Tick(inputHandler.ShootBool);
    }

    private void OnTriggerEnter(Collider other)
    {
        PickUpArea pickUpArea;
        if (other.gameObject.TryGetComponent<PickUpArea>(out pickUpArea))
        {
            var pickUpGunType = pickUpArea.GetPickUpType();
            
            for(int i = 0;i < gunList.Count;i++) 
            {
                if (gunList[i].GetType() == pickUpGunType)
                    gunList[i].AddAmmo();
            }
        }
    }
}
