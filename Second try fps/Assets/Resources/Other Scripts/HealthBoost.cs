using UnityEngine;

[RequireComponent (typeof(Collider))]
public class HealthBoost : MonoBehaviour
{
    [SerializeField] private float _healthAmount;
    [SerializeField] private float _resetTime;

    private MeshRenderer[] children;
    private Collider collider;

    private void Awake()
    {
        children = new MeshRenderer[3];
        children = GetComponentsInChildren<MeshRenderer>();
        
        collider = GetComponent<Collider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.transform.GetComponent<PlayerHealth>().RestoreHealth(_healthAmount);

            foreach (var child in children)
                child.gameObject.SetActive(false);

            collider.enabled = false;

            Invoke(nameof(EnableHealing), _resetTime);
        }
    }

    private void EnableHealing()
    {
        foreach (var child in children)
            child.gameObject.SetActive(true);

        collider.enabled = true;
    }
}
