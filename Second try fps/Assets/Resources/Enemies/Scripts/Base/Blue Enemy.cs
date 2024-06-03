using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(Detector), typeof(NavMeshAgent),typeof(EnemyHealth))]
public class BlueEnemy : Enemy, IShootable
{
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private PickUpArea _pickUpArea;
    [SerializeField] private Rifle gun;

    public Image stateImage;

    private NavMeshAgent agent;
    private EnemyHealth health;
    
    private Detector detector;
    private StateMachine stateMachine;

    private Vector3 idlePosition;

    public NavMeshAgent GetAgentComp()
    {
        return agent;
    }
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

        gun.SetRayCastOrigin(transform.Find("Eye Level"));


        agent.speed = _enemyData.Speed;
        idlePosition = transform.position;
    }

    private void InitializeStateMachine()
    {
        var idle = new IdleState(this, idlePosition, stateImage);
        var combat = new BlueCombat(this, stateImage);
        var chase = new ChasePlayer(agent, stateImage);

        stateMachine = new StateMachine();

        stateMachine.AddAnyTransition(combat, () => detector.IsPlayerInRoom);
        stateMachine.AddAnyTransition(idle, () => 
                !detector.IsPlayerInRoom && !detector.HasPlayerRanAway);
        stateMachine.AddTransition(combat, chase, () => detector.HasPlayerRanAway);
    }

    public bool IsOutOfAmmo()
    {
        return gun.IsOutOfAmmo;
    }
    public void Reload()
    {
        gun.AddAmmo();
    }

    public override void GoTo(Vector3 position)
    {
        agent.SetDestination(position);
        //List<Transform> points = pathfinder.FindPathTo(position);
        //GoPath(points);
    }

    private IEnumerator GoPath(List<Transform> points)
    {
        int currentPointInd = 0;
        bool arrived = false;

        agent.SetDestination(points[currentPointInd].position);
        Debug.DrawRay(points[currentPointInd].position, Vector3.up, Color.blue, 15f);

        while (arrived)
        {
            if(agent.remainingDistance < 0.5f)
            {
                currentPointInd++;
                if (points.Count == currentPointInd)
                    arrived = true;
                else
                {
                    Debug.DrawRay(points[currentPointInd].position, Vector3.up, Color.blue, 15f);
                    agent.SetDestination(points[currentPointInd].position);
                }
            }

            yield return null;
        }
    }

    public void SpeedUpBy(float speed)
    {
        agent.speed += speed;
    }

    public void Shoot()
    {
        gun.Tick(true);
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

    private void OnDisable()
    {
        health.OnHealthZero -= Die;
    }
}
