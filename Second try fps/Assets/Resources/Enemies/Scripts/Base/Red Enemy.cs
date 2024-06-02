using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Detector), typeof(NavMeshAgent), typeof(EnemyHealth))]
public class RedEnemy : Enemy, IShootable
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private PickUpArea _pickUpArea;
    [SerializeField] private Axe axe;

    public Image stateImage;

    private NavMeshAgent agent;
    private EnemyHealth health;
    private Detector detector;
    private int attackTriggerInt = Animator.StringToHash("AttackTr");
    private Animator animator;

    private StateMachine stateMachine;

    private Vector3 idlePosition;
    private float lastAttackTime;

    private void Awake()
    {
        CollectData();

        InitializeStateMachine();
    }
    private void CollectData()
    {
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponent<Detector>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        

        agent.speed = _enemyData.Speed;
        idlePosition = transform.position;
    }

    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine();

        var idle = new IdleState(this, idlePosition, stateImage);
        var combat = new RedCombat(this,  _enemyData.AttackDistance, stateImage);
        var chase = new ChasePlayer(agent, stateImage);

        stateMachine.AddAnyTransition(idle, () =>
                !detector.IsPlayerInRoom && !detector.HasPlayerRanAway);
        stateMachine.AddAnyTransition(combat, () => detector.IsPlayerInRoom);
        stateMachine.AddTransition(combat, chase, () => detector.HasPlayerRanAway);
        stateMachine.AddTransition(chase, combat, chase.CaughtUpWithPlayer);
        stateMachine.AddTransition(chase,idle,chase.LostPlayer);
    }

    public override void GoTo(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 2f, agent.areaMask))
            agent.SetDestination(hit.position);
        else
        {
            Debug.LogError($"{gameObject.name} can not reach position {position}");
            Debug.DrawRay(position, Vector3.up * 5f, Color.red,2f);
        }
    }

    private void OnEnable()
    {
        health.OnHealthZero += Die;
    }

    private void FixedUpdate()
    {
        stateMachine?.Tick();    
    }

    public void Attack()
    {
        if (Time.time > lastAttackTime + _enemyData.AttackSpeed)
        {
            animator.SetTrigger(attackTriggerInt);
            lastAttackTime = Time.time;
        }
    }
    //anim event
    public void StartAttack()
    {
        axe.EnableDamage();
    }
    //anim event
    public void FinishAttack()
    {
        axe.DisableDamage();
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

    private void OnDisable()
    {
        health.OnHealthZero -= Die;
    }
}
