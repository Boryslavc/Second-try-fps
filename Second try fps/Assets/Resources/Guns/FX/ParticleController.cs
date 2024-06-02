using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void PlayParticle()
    {
        particle.Play();
    }

    private void OnParticleSystemStopped()
    {
        ObjectPooler.ReturnGameObject(this);
    }
}
