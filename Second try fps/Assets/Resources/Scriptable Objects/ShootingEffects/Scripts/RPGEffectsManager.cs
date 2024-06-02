using UnityEngine;

[CreateAssetMenu(fileName = "Rocket FXs", menuName = "GunSO/Effects Managers/RPG")]
public class RPGEffectsManager : EffectManager
{
    [SerializeField] private ParticleController _explosionEffect;
    [SerializeField] private ParticleController _shotEffect;

    public void PlayShotEffect(Vector3 position, Quaternion rotation)
    {
        var shot = ObjectPooler.ProvideObject(_shotEffect, position,
            rotation, ObjectPooler.PoolType.Particle); 

        if(shot is ParticleController controller )
        {
            controller.PlayParticle();
        }
    }


    public void PlayExplosionEffect(Vector3 position, Quaternion rotation)
    {
        var explosion = ObjectPooler.ProvideObject(_explosionEffect, position,
            rotation, ObjectPooler.PoolType.Particle);

        if (explosion is ParticleController controller)
        {
            controller.PlayParticle();
        }
    }

}
