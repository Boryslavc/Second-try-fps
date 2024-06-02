using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent),typeof(SphereCollider),typeof(AudioSource))]
public class Kamikaze : MonoBehaviour, IShootable
{
    [SerializeField] private RPGEffectsManager _explosionEffect;
    [SerializeField] private GunSoundSO _soundSO;
    [SerializeField] private float _speed;
    [SerializeField] private float _updateRate;
    [SerializeField] private float _damage;
    [SerializeField] private float _explosionRadius = 3f;
    [SerializeField] private float _maxHealth;

    private SphereCollider collider;
    private NavMeshAgent agent;
    private AudioSource _tickingAudio;
    private Transform player;

    private float currentHealth;
    private LayerMask explodableLayers;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        collider = GetComponent<SphereCollider>();
        player = FindAnyObjectByType<PlayerMovement>().transform;
        _tickingAudio = GetComponent<AudioSource>();
        agent.speed = _speed;

        explodableLayers = LayerMask.GetMask("Enemy", "Player");

        currentHealth = _maxHealth;
    }

    public void FollowPlayer()
    {
        _tickingAudio.Play();

        agent.SetDestination(player.position);
        InvokeRepeating(nameof(UpdateDestination), 1f, _updateRate);
    }
    private void UpdateDestination()
    {
        if (this.gameObject.activeSelf) 
            agent.SetDestination(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            ExplodeSelf();
        }
    }

    public void TakeBullet(float damage)
    {
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            ExplodeSelf();
        }
    }

    private void ExplodeSelf()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius, explodableLayers);

        foreach (Collider collider in colliders)
        {
            if(collider.gameObject.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            }
        }

        _soundSO.PlayShotSound(transform.position);
        _explosionEffect.PlayExplosionEffect(this.transform.position, Quaternion.identity);
        ObjectPooler.ReturnGameObject(this);
        _tickingAudio.Stop();
    }
}
