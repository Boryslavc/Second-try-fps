using System;
using System.Collections;
using UnityEngine;

public class Rifle : Gun
{
    [Header("Weapon settings")]
    [SerializeField] private WeaponConfig _weaponInfo;
    [SerializeField] private GunSoundSO _soundSO;
    [SerializeField] private RifleEffectsManager _effectsManager;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private LayerMask _ignoredLayers;

    [Header("Burst parameters")]
    [SerializeField] private float _burstDelay;
    [SerializeField] private float _burstSize;

    
    private bool isOnEnemy;
    private Vector3 extraSpread = new Vector3(0.03f, 0.02f, 0f);
    
    private float lastShootTime;
    private float initialClickTime;
    private float stopShootingTime;
    private bool shotLastFrame;
    private Vector3 spawnRotation;

    private Transform rayCastOrigin;

    private int ammo;
    private AmmoUI ammoUI;
    private Action<int> updateAmmoInterface;

    private void OnEnable()
    {
        ammo = _weaponInfo.MagazineSize;
        ammoUI = GetComponentInChildren<AmmoUI>();
        updateAmmoInterface = ammoUI.UpdateAmmoUI;
    }

    private void Start()
    {
        updateAmmoInterface(ammo);
        CheckShooter();
        if (rayCastOrigin != null)
            spawnRotation = transform.localRotation.eulerAngles;
    }
    private void CheckShooter()
    {
        if(IsNotAPickUp())
        {
            var enemy = GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                isOnEnemy = true;
                _ignoredLayers |= 1 << transform.parent.gameObject.layer;
            }
            else
            {
                //                     rifle -> camera -> player
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

    public bool IsOutOfAmmo { get { return ammo == 0; } } 
    public override void AddAmmo()
    {
        ammo += _weaponInfo.MagazineSize;
        updateAmmoInterface(ammo);
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
        if(CanShoot())
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

            lastShootTime = Time.time;
            StartCoroutine(ShootBurst());
        }
    }
    private bool CanShoot()
    {
        return lastShootTime + _weaponInfo.TimeBetweenShots < Time.time
            && ammo > 0;
    }

    private IEnumerator ShootBurst()
    {
        var wait = new WaitForSeconds(_burstDelay);

        for(int i = 0; i < _burstSize; i++)
        {
            ammo--;
            updateAmmoInterface(ammo);

            PlayShotEffect();

            _soundSO.PlayShotSound(transform.position);

            var spread = _weaponInfo.GetSpread(Time.time - initialClickTime);
            if (isOnEnemy)
            {
                spread.x += UnityEngine.Random.Range(-extraSpread.x, extraSpread.x);
                spread.y += UnityEngine.Random.Range(-extraSpread.y, extraSpread.y);
            }

            Vector3 direction = _bulletSpawnPoint.forward;

            RaycastHit hit;
            
            if (Physics.Raycast(rayCastOrigin.position, direction + spread, out hit,
                _weaponInfo.Distance, ~_ignoredLayers))
            {
                //DebugMethod(hit, direction);

                transform.forward += transform.TransformDirection(spread);

                var trail = SetUpTrail();

                StartCoroutine(SpawnTrail(trail, hit));

                var target = hit.transform.GetComponent<IShootable>();

                if (target != null)
                {
                    target.TakeBullet(_weaponInfo.Damage);
                }
            }

            yield return wait;
        }
    }
    private void DebugMethod(RaycastHit hit, Vector3 dir)
    {
        float magn = Vector3.Distance(rayCastOrigin.position, hit.point);
        Debug.DrawRay(rayCastOrigin.position, dir * magn, Color.cyan, 3f);
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
        trail.gameObject.transform.position = _bulletSpawnPoint.position;
        trail.gameObject.transform.rotation = _bulletSpawnPoint.rotation;

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
