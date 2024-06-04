using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RunToCoverState : IState
{
    private BlueEnemy blue;

    private CoverArea currentArea;
    private Vector3 coverPosition;

    private LayerMask mask = LayerMask.GetMask("CoverArea");

    private Image stateImage;

    public RunToCoverState(BlueEnemy blue, Image image)
    {
        this.blue = blue;

        stateImage = image;
    }


    public bool Arrived
    {
        get
        {
            if (currentArea == null)
                FindArea();
            return Vector3.Distance(blue.GetAgentComp().destination, blue.transform.position) < 5f
                && currentArea.IsPlayerNearby == true;
        }
    }
    // does more than one thing REFACTOR
    private void FindArea()
    {
        Collider[] col = Physics.OverlapSphere(blue.transform.position, 3f, mask);

        foreach(var c in col)
        {
            if(c.TryGetComponent(out CoverArea area))
            {
                currentArea = area;
                break;
            }
        }

        if (NavMesh.SamplePosition(currentArea.transform.position,
            out NavMeshHit hit, 5f, blue.GetAgentComp().areaMask))
            blue.GoTo(hit.position);
    }

    public void OnEnter()
    {
        var areas = LevelGeometry.Instance.GetCoverAreasAroundPlayer();
        ComputePosition(areas);

        blue.SpeedUpBy(2f);

        stateImage.color = Color.green;
    }
    private void ComputePosition(List<CoverArea> areas)
    {
        if(areas.Count > 0)
        {
            // possible filters to sort out the areas
            int randNumb = Random.Range(0, areas.Count);

            coverPosition = areas[randNumb].transform.position;
            currentArea = areas[randNumb];

            blue.GoTo(coverPosition);
        }
    }

    public void Tick()
    {

    }

    public void OnExit()
    {
        blue.SpeedUpBy(-2f);
    }  
}
