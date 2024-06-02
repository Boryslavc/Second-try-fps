using System.Collections;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
public class LineOfSightChecker : MonoBehaviour
{
    [SerializeField] private float _FOV;
    [SerializeField] private float _alertSensitivity;
    [SerializeField] private LayerMask _sightLayers;
    [SerializeField] private Transform _eyes;

    public delegate void GainSightEvent();
    public delegate void LoseSightEvent();

    public GainSightEvent OnSightGained;
    public LoseSightEvent OnSightLost;

    private SphereCollider collider;
    private Coroutine checkLineOfSightCoroutine;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            checkLineOfSightCoroutine = StartCoroutine(CheckCoroutine(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            OnSightLost?.Invoke();

            if(checkLineOfSightCoroutine != null)
                StopCoroutine(checkLineOfSightCoroutine);
        }
    }

    private IEnumerator CheckCoroutine(Transform target)
    {
        WaitForSeconds wait = new WaitForSeconds(_alertSensitivity);

        while(true)
        {
            CheckLineOfSight(target);
            yield return wait;
        }
    }
    private void CheckLineOfSight(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;

        transform.forward = direction;
        
        RaycastHit hit;
        if(Physics.Raycast(_eyes.transform.position, direction, out hit, collider.radius, _sightLayers))
        {
            //DebugMethod(direction);
            if(hit.transform.CompareTag("Player"))
            {
                OnSightGained?.Invoke();
            }
            else
            {
                OnSightLost?.Invoke();
            }
        }
    }

    private void DebugMethod(Vector3 direction)
    {
        Debug.DrawRay(_eyes.transform.position, direction * 5f, Color.yellow, 1f);
    }
}
