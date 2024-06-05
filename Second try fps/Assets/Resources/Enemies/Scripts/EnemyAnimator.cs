using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimator : MonoBehaviour
{
    
    [SerializeField] private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private int attackTrHash = Animator.StringToHash("AttackTr");

    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (_navMeshAgent.velocity.magnitude > 0)
        {
            _animator.SetBool("IsRunning", true);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
        }
    }

    public void SetAttack()
    {
        _animator.SetTrigger(attackTrHash);
    }
}
