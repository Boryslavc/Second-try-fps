using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public Action<Enemy> OnDied;
    public abstract void GoTo(Vector3 position);
}
