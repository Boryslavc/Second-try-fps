using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class YellowCombat : IState
{
    private NavMeshAgent agent;
    private Image image;

    private Kamikaze kamikaze;
    private float timeRate = 20f;
    private float lastReleaseTime;

    public YellowCombat(NavMeshAgent agent, Kamikaze kamikaze, Image image)
    {
        this.agent = agent;
        this.image = image;
        this.kamikaze = kamikaze;
    }

    public void OnEnter()
    {
        image.color = Color.red;
        agent.isStopped = true;
    }

    public void Tick()
    {
        if(lastReleaseTime + timeRate  < Time.time)
        {
            ReleaseKamikaze();
            lastReleaseTime = Time.time;
        }
    }
 
    private void ReleaseKamikaze()
    {
        if(NavMesh.SamplePosition(agent.transform.position, out NavMeshHit hit, 3f, agent.areaMask))
        {
            var bomb = ObjectPooler.ProvideObject(kamikaze, hit.position,
                kamikaze.transform.rotation, ObjectPooler.PoolType.GameObject);

            if(bomb is Kamikaze kamikazeBomb)
            {
                kamikazeBomb.FollowPlayer();
            }
        }
        else
        {
            Debug.LogError($"No spare space found around {agent.transform.position} to place a prefab.");
        }
    }
    
    public void OnExit()
    {
        agent.isStopped = false;
    }
}
