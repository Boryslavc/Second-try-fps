using System;
using System.Collections;
using UnityEngine;

public class SubMachineGun : Gun
{
    [SerializeField] private WeaponConfig _weaponInfo;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private GunSoundSO _soundSO;
    [SerializeField] private RifleEffectsManager _effectsManager;
    [SerializeField] private LayerMask _ignoredLayers;

    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool shotLastFrame;
    private Transform rayCastOrigin;
    private Vector3 spawnRotation;
    
    private int ammo;
    private AmmoUI ammoUI;
    private Action<int> updateAmmoInterface;

    private bool isOnEnemy;
    private Vector3 extraSpread = new Vector3(0.01f, 0.01f, 0.01f);


    private void OnEnable()
    {
        ammo = _weaponInfo.MagazineSize;
        ammoUI = GetComponentInChildren<AmmoUI>();
        updateAmmoInterface += ammoUI.UpdateAmmoUI;
    }

    private void Start()
    {
        CheckShooter();
        updateAmmoInterface(ammo);
        if(rayCastOrigin != null)
            spawnRotation = transform.localRotation.eulerAngles;
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

    public bool IsOutOfAmmo() => ammo == 0;
    public override void AddAmmo()
    {
        ammo += _weaponInfo.MagazineSize;
        updateAmmoInterface(ammo);
    }
        
    public override void SetRayCastOrigin(Transform point)
    {
        rayCastOrigin = point;
    }

    public override void Tick(bool wantsToShoot)
    {
        transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                Quaternion.Euler(spawnRotation),
                Time.deltaTime * _weaponInfo.RecoilRecoverySpeed
            );

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

            var spread = _weaponInfo.GetSpread(Time.time - initialClickTime);
            Vector3 direction = _bulletSpawnPoint.forward;

            RaycastHit hit;
            if (Physics.Raycast(rayCastOrigin.position, direction + spread, out hit, _weaponInfo.Distance, ~_ignoredLayers))
            {
                //DebugMethod(hit, direction);

                transform.forward += transform.TransformDirection(spread);

                var trail = SetUpTrail();

                StartCoroutine(SpawnTrail(trail, hit));

                lastShootTime = Time.time;

                IShootable shootable;
                if(hit.transform.TryGetComponent<IShootable>(out  shootable))
                {
                    shootable.TakeBullet(_weaponInfo.Damage);
                }
            }
        }
    }
    private void DebugMethod(RaycastHit hit, Vector3 dir)
    {
        float magn = Vector3.Distance(rayCastOrigin.position, hit.point);
        Debug.DrawRay(rayCastOrigin.position, dir * magn, Color.yellow, 3f);
    }
    private bool CanShoot()
    {
        return lastShootTime + _weaponInfo.TimeBetweenShots < Time.time
            && ammo > 0;
    }

    private void PlayShotEffect()
    {
        var position = _bulletSpawnPoint.position;
        var rotation = _bulletSpawnPoint.rotation;

        _effectsManager.PlayShotEffect(position, rotation);
    }

    private TrailRenderer SetUpTrail()
    {
        var trail = _effectsManager.GetBulletTrail();

        trail.gameObject.SetActive(true);
        trail.transform.position = _bulletSpawnPoint.position;
        trail.transform.rotation = _bulletSpawnPoint.rotation;

        return trail;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;

        Vector3 startPos = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPos, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hit.point;

        PlayImpactEffect(hit.point);

        trail.gameObject.SetActive(false);
    }

    private void PlayImpactEffect(Vector3 position)
    {
        _effectsManager.PlayBulletImpactEffect(position, Quaternion.identity);
    }
}
