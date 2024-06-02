using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent),typeof(EnemyHealth))]
public class YellowEnemy : Enemy, IShootable
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private PickUpArea _pickUpArea;
    [SerializeField] private Kamikaze _kamikazePrefab;

    public Image stateImage;

    private NavMeshAgent agent;
    private EnemyHealth health;

    private LineOfSightChecker lineOfSightChecker;
    private Detector detector;

    private StateMachine stateMachine;
    private bool isPlayerInSight = false;

    private Vector3 idlePosition;

    public Vector3 GetStartingPosition()
    {
        return idlePosition;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        detector = GetComponent<Detector>();

        lineOfSightChecker = GetComponentInChildren<LineOfSightChecker>();
        lineOfSightChecker.OnSightGained += () => isPlayerInSight = true;
        lineOfSightChecker.OnSightLost = () => isPlayerInSight = false;

        agent.speed = _enemyData.Speed;
        idlePosition = transform.position;

        InitializeStateMachine();
    }
    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine();

        var hide = new HideFromPlayer(agent, stateImage);
        var combat = new YellowCombat(agent, _kamikazePrefab, stateImage);

        stateMachine.AddAnyTransition(hide, () => isPlayerInSight);
        stateMachine.AddTransition(hide, combat , () =>  !isPlayerInSight);
    }

    private void OnEnable()
    {
        health.OnHealthZero += Die;
    }


    private void FixedUpdate()
    {
        stateMachine?.Tick();
    }

    public void TakeBullet(float damage)
    {
        health.TakeDamage(damage);
    }

    private void Die()
    {
        OnDied?.Invoke(this);
        var pickup = ObjectPooler.ProvideObject(_pickUpArea, transform.position,
            Quaternion.identity, ObjectPooler.PoolType.GameObject);
        
        if(pickup is PickUpArea area)
        {
            area.IsPickableOnce = true;
        }

        ObjectPooler.ReturnGameObject(this);
    }
    
    public bool IsMoving()
    {
        return agent.hasPath;
    }
    public override void GoTo(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 1.5f, agent.areaMask))
            agent.SetDestination(position);
        else
            Debug.LogError($"{gameObject.name} can not reach position {position}");
    }

    private void OnDisable()
    {
        health.OnHealthZero -= Die;
    }
}
