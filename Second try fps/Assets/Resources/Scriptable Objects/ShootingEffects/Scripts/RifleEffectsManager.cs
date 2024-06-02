using UnityEngine;

[CreateAssetMenu(fileName = "Rifle Fxs", menuName = "GunSO/Effects Managers/Auto-Rifle")]
public class RifleEffectsManager : EffectManager
{
    [SerializeField] private ParticleController _shotEffectPrefab;
    [SerializeField] private ParticleController _imapctEffectPrefab;
    [SerializeField] private TrailRenderer _trailPrefab;


    public void PlayShotEffect(Vector3 position, Quaternion rotation)
    {
        var shot = ObjectPooler.ProvideObject(_shotEffectPrefab, position,
            rotation, ObjectPooler.PoolType.Particle);
        
        if(shot is ParticleController controller)
        {
            controller.PlayParticle();
        }
    }

    public TrailRenderer GetBulletTrail()
    {
        var trail = ObjectPooler.ProvideObject(_trailPrefab,Vector3.zero,
            Quaternion.identity, ObjectPooler.PoolType.Particle);

        if(trail is TrailRenderer trueTrail)
        {
            return trueTrail;
        }
        else
        {
            return null;
        }
    }

    public void PlayBulletImpactEffect(Vector3 position, Quaternion rotation)
    {
        var effect = ObjectPooler.ProvideObject(_imapctEffectPrefab,
            position, rotation, ObjectPooler.PoolType.Particle);

        if (effect is ParticleController controller)
        {
            controller.PlayParticle();
        }
    }
}
