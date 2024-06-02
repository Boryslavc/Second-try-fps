using System;
using UnityEngine;

public class RocketLauncher : Gun
{
    [Header("Weapon settings")]
    [SerializeField] private WeaponConfig _weaponInfo;
    [SerializeField] private GunSoundSO _soundSO;
    [SerializeField] private RPGEffectsManager _effectManager;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private LayerMask _ignoredLayers;

    [Header("Rocket launcher settings")]
    [SerializeField] private Misle _mislePrefab;


    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool shotLastFrame;
    private Transform rayCastOrigin;
    private int ammo;

    private bool isOnEnemy;
    private Vector3 extraSpread = new Vector3(0.01f, 0.01f, 0.01f);

    private AmmoUI ammoUI;
    private Action<int> updateAmmoInterface;
    private GameObject attachedRocket;

    private void OnEnable()
    {
        ammo = _weaponInfo.MagazineSize;
        ammoUI = GetComponentInChildren<AmmoUI>();
        updateAmmoInterface = ammoUI.UpdateAmmoUI;
    }

    private void Start()
    {
        attachedRocket = transform.GetChild(0).gameObject;
        updateAmmoInterface(ammo);
        CheckShooter();
    }
    private void CheckShooter()
    {
        if (IsNotAPickUp())
        {
            var enemy = GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                isOnEnemy = true;
                _ignoredLayers |= 1 << transform.parent.gameObject.layer;
            }
            else
            {
                _ignoredLayers |= 1 << transform.parent.transform.parent.gameObject.layer;
            }
        }
    }
    private bool IsNotAPickUp()
    {
        return !transform.parent.TryGetComponent<PickUpArea>(out PickUpArea area);
    }

    public override void SetRayCastOrigin(Transform point)
    {
        rayCastOrigin = point;
    }

    public bool IsOutOfAmmo() => ammo == 0;
    public override void AddAmmo()
    {
        ammo += _weaponInfo.MagazineSize;
        updateAmmoInterface(ammo);
    }

    public override void Tick(bool wantsToShoot)
    {
        if (wantsToShoot)
        {
            shotLastFrame = true;
            Shoot();
        }
        else
        {
            shotLastFrame = false;
            stopShootingTime = Time.time;
        }
    }

    private void Shoot()
    {
        if (Time.time - lastShootTime - _weaponInfo.TimeBetweenShots > Time.deltaTime)
        {
            float lastDuration = Mathf.Clamp(
                0,
                (stopShootingTime - initialClickTime),
                _weaponInfo.MaxSpreadTime
            );
            float lerpTime = (_weaponInfo.RecoilRecoverySpeed - (Time.time - stopShootingTime))
                             / _weaponInfo.RecoilRecoverySpeed;

            initialClickTime = Time.time - Mathf.Lerp(0, lastDuration, Mathf.Clamp01(lerpTime));
        }

        if (CanShoot())
        {
            ammo--;
            updateAmmoInterface(ammo);

            PlayShotEffect();

            _soundSO.PlayShotSound(transform.position);

            Vector3 direction = _bulletSpawnPoint.forward;
            Vector3 spread = GetSpread();

            RaycastHit hit;
            if (Physics.Raycast(rayCastOrigin.position, direction + spread, out hit, _weaponInfo.Distance, ~_ignoredLayers))
            {
                lastShootTime = Time.time;

                attachedRocket.SetActive(false);
                Invoke(nameof(EnableRocket), _weaponInfo.TimeBetweenShots);

                var rocket = SetMisleUp();
                rocket.SetFlying(_effectManager, hit);
            }
        }
    }
    private bool CanShoot()
    {
        return lastShootTime + _weaponInfo.TimeBetweenShots < Time.time
            && ammo > 0;
    }

    private void PlayShotEffect()
    {
        var position = _bulletSpawnPoint.position;

        _effectManager.PlayShotEffect(position, Quaternion.identity);
    }

    private Vector3 GetSpread()
    {
        var spread = _weaponInfo.GetSpread();

        if (isOnEnemy)
        {
            spread += extraSpread;
        }

        return spread.normalized;
    }
    private Misle SetMisleUp()
    {
        var launchedRocket = ObjectPooler.ProvideObject(_mislePrefab,
            _bulletSpawnPoint.position, attachedRocket.transform.rotation); 

        if(launchedRocket is Misle misle)
        {
            return misle;
        }

        return null;

        //return launchedRocket.GetComponent<Misle>();
    }

    private void EnableRocket()
    {
        attachedRocket.SetActive(true);
    }

    private void PlayImpactEffect(RaycastHit hit)
    {
        var playPosition = hit.point + hit.normal;

        _effectManager.PlayExplosionEffect(playPosition, Quaternion.identity);
    }


}

