using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class HideFromPlayer : IState
{
    private NavMeshAgent agent;
    private Image image;
    private Transform player;

    private float baseOffset = 3f;
    
    private LayerMask hidableLayer;

    // bigger size means more potential hiding places but slower computing
    private Collider[] colliders = new Collider[10]; 
    
    public HideFromPlayer(NavMeshAgent agent, Image image)
    {
        this.agent = agent;
        this.image = image;

        hidableLayer = LayerMask.GetMask("Hideable");
        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;
    }

    public void OnEnter()
    {
        image.color = Color.yellow;
    }

    public void Tick()
    {
        if (!agent.hasPath)
        {
            int hits = Physics.OverlapSphereNonAlloc(agent.transform.position, 15f,
                colliders, hidableLayer);

            if (colliders[0] == null)
                Debug.LogError($"No hide spots around {agent.transform.position}");
            else
            {
                for (int i = 0; i < hits; i++)
                {
                    if (NavMesh.SamplePosition(colliders[i].transform.position, out NavMeshHit hit, baseOffset, agent.areaMask))
                    {
                        if (NavMesh.FindClosestEdge(hit.position, out hit, agent.areaMask))
                        {
                            Vector3 dir = (player.position - hit.position).normalized;
                            //DebugMethod(hit, dir);

                            if (Vector3.Dot(hit.normal, dir) < 0)
                            {
                                agent.SetDestination(hit.position);
                            }
                            else
                            {
                                Vector3 offset = (colliders[i].transform.position - player.position).normalized * 4;
                                if (NavMesh.SamplePosition(colliders[i].transform.position + offset,
                                    out NavMeshHit hit2, baseOffset, agent.areaMask))
                                {
                                    if (NavMesh.FindClosestEdge(hit2.position, out hit2, agent.areaMask))
                                    {
                                        Vector3 dir2 = (player.position - hit2.position).normalized;
                                        //DebugMethod(hit, dir2);

                                        if (Vector3.Dot(hit2.normal, dir2) < 0)
                                        {
                                            agent.SetDestination(hit2.position);
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogError($"Failed to find edge around {hit.position}");
                                        Debug.DrawRay(hit.position, Vector3.up * 10f, Color.yellow, 2f);
                                    }
                                }
                                else
                                {
                                    Debug.LogError($"Failed to find NavMesh point around {colliders[i].transform.position}");
                                    Debug.DrawRay(hit.position, Vector3.up * 10f, Color.yellow, 2f);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError($"Failed to find edge around {hit.position}");
                            Debug.DrawRay(hit.position, Vector3.up * 10f, Color.yellow, 2f);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Failed to find NavMesh point around {colliders[i].transform.position}");
                        Debug.DrawRay(hit.position, Vector3.up * 10f, Color.yellow, 2f);
                    }
                }
            }
        }
    }



    private void DebugMethod(NavMeshHit hit, Vector3 direction)
    {
        Debug.DrawRay(hit.position, direction * 5f, Color.black, 2f);
        Debug.DrawRay(hit.position, hit.normal * 5f, Color.yellow, 2f);
    }

    public void OnExit()
    {
        
    }
}
