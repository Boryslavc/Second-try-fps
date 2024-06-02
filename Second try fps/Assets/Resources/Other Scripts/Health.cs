using UnityEngine;

public abstract class Health : MonoBehaviour
{
    public abstract void TakeDamage(float damage);
    public abstract void RestoreHealth(float gained);
}
