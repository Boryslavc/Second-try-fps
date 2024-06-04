using System.Collections.Generic;
using UnityEngine;

public class MovingStructureNode : MonoBehaviour
{
    [SerializeField] private Color gizmosColor;
    [SerializeField] private List<MovingStructureNode> connectedNodes;

    public List<Vector3> forwardVectors { get; private set; }

    public List<MovingStructureNode> GetConnectedNodes()
    {
        return connectedNodes;
    }

    private void Awake()
    {
        InitForwardVectors();
    }

    private void OnDrawGizmos()
    {
        InitForwardVectors();

        Gizmos.color = gizmosColor;
        Gizmos.DrawSphere(transform.position, 0.5f);
        for (int i = 0; i < connectedNodes.Count; i++)
            Gizmos.DrawRay(transform.position, forwardVectors[i] * 3);
    }

    private void InitForwardVectors()
    {
        forwardVectors = new List<Vector3>(connectedNodes.Count);

        for (int i = 0; i < connectedNodes.Count; i++)
            forwardVectors.Add((connectedNodes[i].transform.position - transform.position).normalized);
    }
}