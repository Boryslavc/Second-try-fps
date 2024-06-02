using UnityEngine;

[RequireComponent (typeof(Collider))]
public class Axe : MonoBehaviour
{
    [SerializeField] private float _damage;

    private bool dealsDamage = false;

    private void OnTriggerEnter(Collider other)
    {
        Health someonesHealth;
        if (other.gameObject.TryGetComponent<Health>(out someonesHealth) && dealsDamage)
            someonesHealth.TakeDamage(_damage);
    }

    public void EnableDamage()
    {
        dealsDamage = true;
    }
    public void DisableDamage()
    {
        dealsDamage = false;
    }
}
