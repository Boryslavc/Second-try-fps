using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Layout", menuName = "Enemies/Enemy Layout")]
public class EnemyLayout : ScriptableObject
{
    [SerializeField]
    private Layout layout = new Layout();

    [ContextMenu("Save current enemy layout")]
    public void SaveCurrentLayout()
    {

        List<Enemy> enemies = new List<Enemy>();

        enemies = GameObject.FindObjectsOfType<Enemy>().ToList();

        if (enemies.Count == 0)
            Debug.LogError("No enemy found on the scene.");

        foreach (Enemy enemy in enemies)
        {
            layout.Add(enemy);
        }
    }

    public Layout GetLayout() { return layout; }
}


[Serializable]
public class Layout
{
    [SerializeField]
    private SerializableDictionary<Type,List<Vector3>> enemiesLayout =
        new SerializableDictionary<Type, List<Vector3>>();

    public void Add(Enemy enemy)
    {
        if (enemiesLayout.ContainsKey(enemy.GetType()))
        {
            Vector3 pos = new Vector3(enemy.transform.position.x,
                enemy.transform.position.y,
                enemy.transform.position.z);
            enemiesLayout.Get(enemy.GetType()).Add(pos);
        }
        else
        {
            enemiesLayout.Add(enemy.GetType(), new List<Vector3>()); 
            Vector3 pos = new Vector3(enemy.transform.position.x,
                enemy.transform.position.y,
                enemy.transform.position.z);
            enemiesLayout.Get(enemy.GetType()).Add(pos);
        }
    }

   public List<Vector3> GetEnemyTypePositions(Enemy enemy)
    {
        return enemiesLayout.Get(enemy.GetType());
    }
}



[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var pair in dictionary)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary = new Dictionary<TKey, TValue>();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Keys and values count mismatch in SerializableDictionary.");
            return;
        }

        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public TValue Get(TKey key)
    {
        return dictionary[key];
    }

    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
    }

    public bool Remove(TKey key)
    {
        return dictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }
}