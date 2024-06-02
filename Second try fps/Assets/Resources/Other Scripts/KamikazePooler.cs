using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazePooler: MonoBehaviour 
{
    [SerializeField] private Kamikaze prefab;
    [SerializeField] private float sizePool;

    private List<Kamikaze> pool;

    private void Start()
    {
        if (prefab != null)
        {
            pool = new List<Kamikaze>();
            for (int k = 0; k < sizePool; k++)
            {
                var trail = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                trail.gameObject.SetActive(false);
                pool.Add(trail);
            }
        }
    }

    public Kamikaze GetObject()
    {
        foreach (var gameObject in pool)
        {
            if (!gameObject.gameObject.activeInHierarchy)
                return gameObject;
        }
        return null;
    }
}
