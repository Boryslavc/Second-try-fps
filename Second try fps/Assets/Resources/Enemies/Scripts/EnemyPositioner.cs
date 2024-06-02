using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnInfo
{
    public readonly Enemy enemyType;
    public readonly int simultaneoslyInTheSceneCount;
    public readonly int maxSpawnCount;

    public int nowInTheSceneCount;
    public int alreadySpawnedCount;

    public EnemySpawnInfo(Enemy enemyType, int minCountAtATime,
        int maxSpawnCount, int alreadySpawnedCount = 0)
    {
        this.enemyType = enemyType;
        this.simultaneoslyInTheSceneCount = minCountAtATime;
        this.maxSpawnCount = maxSpawnCount;
        this.alreadySpawnedCount = alreadySpawnedCount;
    }
}


public class EnemyPositioner : MonoBehaviour
{
    [SerializeField] private List<Transform> _spawnPositions;
    [SerializeField] private List<Enemy> _includeEnemies;
    [SerializeField] private List<int> _maxSpawnCount;
    [SerializeField] private List<int> _spawnedAtATimeCount;
    
    private List<EnemySpawnInfo> _spawnInfos;


    //for later use
    //[SerializeField] private EnemyLayout _enemiesLayout;
    //private int desiredEnemyLayout;

    private Dictionary<Type, EnemySpawnInfo> enemiesAlive = new Dictionary<Type, EnemySpawnInfo>();
    private Transform player;

    private EnemySpawnInfo SpawnInfo(Enemy enemy) => enemiesAlive[enemy.GetType()];

    private void Awake()
    {
        _spawnInfos = new List<EnemySpawnInfo>();

        for(int i = 0; i < _includeEnemies.Count; i++)
        {
            _spawnInfos.Add(new EnemySpawnInfo(_includeEnemies[i],
                _spawnedAtATimeCount[i], _maxSpawnCount[i]));
        }
    }

    public void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>().transform;

        int positionIndex = -1;

        foreach (Enemy enemy in _includeEnemies)
        {
            EnemySpawnInfo spawnRuleEnemy = _spawnInfos.Find((EnemySpawnInfo spawnRule) =>
            enemy.GetType() == spawnRule.enemyType.GetType());

            for (int i = 0; i <= spawnRuleEnemy.simultaneoslyInTheSceneCount; i++)
            {
                positionIndex = (positionIndex + 1) % _spawnPositions.Count;

                Vector3 position = _spawnPositions[positionIndex].position +
                    new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(-3, 3));

                var instance = ObjectPooler.ProvideObject(enemy, position,
                    enemy.transform.rotation, ObjectPooler.PoolType.GameObject);


                if(instance is Enemy enemt)
                {
                    enemt.OnDied += OneDown;
                }

                if (enemiesAlive.ContainsKey(enemy.GetType()))
                {
                    SpawnInfo(enemy).alreadySpawnedCount++;
                    SpawnInfo(enemy).nowInTheSceneCount++;
                }
                else
                {
                    enemiesAlive.Add(enemy.GetType(),new EnemySpawnInfo(enemy,spawnRuleEnemy.simultaneoslyInTheSceneCount,
                        spawnRuleEnemy.maxSpawnCount,1));
                    SpawnInfo(enemy).nowInTheSceneCount++;
                }
            }
        }
    }

    private void OneDown(Enemy enemy)
    {
        SpawnInfo(enemy).nowInTheSceneCount--;

        if (SpawnInfo(enemy).nowInTheSceneCount <= 0)
            PlaceMore(enemy);
    }
    private void PlaceMore(Enemy enemy)
    {
        var ind = UnityEngine.Random.Range(0, _spawnPositions.Count - 1);
        var pos = _spawnPositions[ind];

        if(!IsPlayerAround(pos.position))
        {
            SpawnEnemy(enemy, pos.position);
        }
        else
        {
            ind = (ind + 1) % _spawnPositions.Count;
            pos = _spawnPositions[ind];

            SpawnEnemy(enemy, pos.position);
        }
    }

    private void SpawnEnemy(Enemy enemy, Vector3 position)
    {
        for (int i = 0; i < SpawnInfo(enemy).simultaneoslyInTheSceneCount; i++)
        {
            if (SpawnInfo(enemy).alreadySpawnedCount < SpawnInfo(enemy).maxSpawnCount)
            {
                var instance = ObjectPooler.ProvideObject(enemy, position,
                    enemy.transform.rotation, ObjectPooler.PoolType.GameObject);

                SpawnInfo(enemy).nowInTheSceneCount++;
                SpawnInfo(enemy).alreadySpawnedCount++;

                if (instance is Enemy enemt)
                {
                    enemt.OnDied += OneDown;
                }
            }
        }
    }
    private bool IsPlayerAround(Vector3 position)
    {
        var distanceToPlayer = (player.position - position).magnitude;

        return distanceToPlayer < 4f;
    }
}
