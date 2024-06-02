using UnityEngine;
using System;

public class PickUpArea : MonoBehaviour
{
    [SerializeField] private Gun _pickUp;
    [SerializeField] private float _resetDelay;

    public bool IsPickableOnce = false;

    private Gun weapon;
    private Collider collider;

    public Type GetPickUpType() => _pickUp.GetType(); 

    private void Awake()
    {
        weapon = Instantiate(_pickUp, transform);
        weapon.transform.position = transform.position;
        weapon.transform.localScale = new Vector3(2f, 2f, 2f);
        collider = GetComponent<Collider>();
    }

    private void Update()
    {
        weapon.transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (!IsPickableOnce)
                DisableWeapon();
            else
                Destroy(gameObject);
        }
    }
    private void DisableWeapon()
    {
        weapon.gameObject.SetActive(false);
        collider.enabled = false;

        Invoke(nameof(EnableWeapon), _resetDelay);
    }
    private void EnableWeapon()
    {
        weapon.gameObject.SetActive(true);
        collider.enabled = true;
    }
}
