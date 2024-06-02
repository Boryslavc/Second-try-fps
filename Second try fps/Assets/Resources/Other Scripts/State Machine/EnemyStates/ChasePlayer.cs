using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ChasePlayer : IState
{
    private NavMeshAgent agent;
    private Transform player;

    private Vector3 destination;

    private LayerMask ignoreLayers;

    private Image image;

    public ChasePlayer(NavMeshAgent agent, Image image)
    {
        this.agent = agent;
        this.image = image;

        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;

        SetUpIgnoreLayer();
    }
    private void SetUpIgnoreLayer()
    {
        var agentLayer = agent.gameObject.layer;

        for (int i = 0; i < 32; i++)
        {
            if (Physics.GetIgnoreLayerCollision(agentLayer, i))
                ignoreLayers |= 1 << i; 
        }
    }

    public void OnEnter()
    {
        destination = player.position;
        agent.SetDestination(destination);
        image.color = Color.cyan;
    }

    public void Tick()
    {
        Vector3 direction = (player.position - agent.transform.position).normalized;

        RaycastHit hit;
        //TO REFACTOR...  closed areas only on the level, so it's gonna hit something anyway
        if (Physics.Raycast(agent.transform.position, direction,
            out hit, 100f, ~ ignoreLayers))
        {
            if(hit.transform.CompareTag("Player"))
            {
                if (destination != hit.transform.position)
                {
                    destination = hit.transform.position;
                    agent.SetDestination(destination);
                }
            }
        }
    }

    public bool CaughtUpWithPlayer()
    {
        // TO REFACTOR... different enemies might require different distance
        return agent.transform.position.HorizontalSquaredDistanceTo(player.position) < 1f;
    }

    public bool LostPlayer()
    {
        return HasArrived() && PlayerIsGone();
    }
    private bool HasArrived()
    {
        return agent.transform.position.HorizontalSquaredDistanceTo(destination) < 1f;
    }
    private bool PlayerIsGone()
    {
        return player.position.HorizontalSquaredDistanceTo(destination) > 4f;
    }

    public void OnExit()
    {
        image.color = Color.gray;
    }
}
