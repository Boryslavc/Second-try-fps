using UnityEngine;

public class Arm : MonoBehaviour
{
    public bool isDamagable = true;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Health>(out Health health))
        {
                health.TakeDamage(35);
        }
    }
}
