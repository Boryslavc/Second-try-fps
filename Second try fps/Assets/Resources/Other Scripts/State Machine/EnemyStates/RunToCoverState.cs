using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RunToCoverState : IState
{
    private BlueEnemy blue;
    private Transform player;

    private CoverArea current;


    private Image stateImage;

    public RunToCoverState(BlueEnemy blue, Image image)
    {
        this.blue = blue;
        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;

        stateImage = image;
    }


    public bool CloseEnoughToPlayer
    {
        get
        {
            if (current == null)
                current = LevelGeometry.Instance.GetCoverAreasAroundPlayer()[0];
            return current.IsPlayerNearby && 
                Vector3.Distance(blue.transform.position, current.transform.position) < 5f;
        }
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
        // possible filters to sort out the areas
        int randNumb = Random.Range(0, areas.Count);
                
        Vector3 pos = areas[randNumb].transform.position;

        if(NavMesh.SamplePosition(pos, out NavMeshHit hit, 3f, blue.GetAgentComp().areaMask))
            blue.GoTo(hit.position);
    }

    public void Tick()
    {

    }

    public void OnExit()
    {
        blue.SpeedUpBy(-2f);
    }  
}
