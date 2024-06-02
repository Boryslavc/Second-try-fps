using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Misle : MonoBehaviour
{
    [SerializeField] private ParticleSystem _smokeTrail;
    [SerializeField] private GunSoundSO _explosionSound;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;

    private ParticleSystem _trail;


    private void OnEnable()
    {
        _trail = Instantiate(_smokeTrail, transform.position, gameObject.transform.rotation);
        _trail.transform.SetParent(transform,false);
        _trail.transform.position = transform.position;
        _trail.Play();
    }

    public void SetFlying(RPGEffectsManager effects, RaycastHit hit)
    {
        StartCoroutine(Fly(hit,effects));
    }
    private IEnumerator Fly(RaycastHit hit, RPGEffectsManager effects)
    {
        Vector3 startPos = transform.position;

        float distance = Vector3.Distance(transform.position, hit.point);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            float distanceCovered = 1 - (remainingDistance / distance);

            transform.position = Vector3.Lerp(startPos, hit.point,
                distanceCovered);
            remainingDistance -= Time.deltaTime * _speed;

            yield return null;
        }
        transform.position = hit.point;

        effects.PlayExplosionEffect(hit.point, Quaternion.identity);

        Explode(transform.position);
    }

    private void Explode(Vector3 centerPoint)
    {
        Collider[] affectedObjects = Physics.OverlapSphere(centerPoint, _explosionRadius);

        List<IShootable> targets = new List<IShootable>();

        foreach (Collider col in affectedObjects)
        {
            IShootable target;
            if (col.transform.TryGetComponent<IShootable>(out target))
            {
                targets.Add(target);
            }

        }

        foreach (var target in targets)
            target.TakeBullet(_damage);

        _explosionSound.PlayShotSound(transform.position);

        ObjectPooler.ReturnGameObject(this);
    }
}
