using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyPooler : MonoBehaviour
{
    [SerializeField] private List<Enemy> _prefabs;
    [SerializeField] private int _poolSize;

    private Dictionary<Type, List<Enemy>> pool = new Dictionary<Type, List<Enemy>>();


    private void Awake()
    {
        pool.Clear();

        foreach (var enemy in _prefabs)
        {
            pool.Add(enemy.GetType(), new List<Enemy>());

            for(int i = 0; i < _poolSize; i++)
            {
                var prefab = Instantiate(enemy, Vector3.zero, Quaternion.identity);
                prefab.gameObject.SetActive(false);
                pool[enemy.GetType()].Add(prefab);
            }
        }
    }

    public Enemy GetEnemyByType(Type type)
    {
        var list = pool[type];

        foreach (var enemy in list)
        {
            if(!enemy.gameObject.activeSelf)
            {
                return enemy;
            }
        }

        Debug.LogError("All enemies are active of type " + type);
        return null;
    }
}
