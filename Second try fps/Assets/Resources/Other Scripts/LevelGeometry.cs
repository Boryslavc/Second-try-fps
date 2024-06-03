using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class LevelGeometry : MonoBehaviour
{
    public static LevelGeometry Instance;

    private List<CoverArea> coverAreas;

    private NavMeshSurface surface;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        FindCoverAreas();

        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }
    private void FindCoverAreas()
    {
        coverAreas = new List<CoverArea>();
        coverAreas = FindObjectsOfType<CoverArea>().ToList();
    }


    public CoverArea GetClosestCoverAreaRelativeTo(Vector3 position)
    {
        float closestDistance = float.PositiveInfinity;
        CoverArea closetsArea = null;

        foreach (var area in coverAreas)
        {
            if(area.HasCoverAvailable())
            {
                float distance = position.HorizontalDistance(area.transform.position); 
                
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closetsArea = area;
                }
            }
        }
        return closetsArea;
    }

    public List<CoverArea> GetCoverAreasAroundPlayer()
    {
        List<CoverArea> closets = new List<CoverArea>();

        foreach(var area in coverAreas)
        {
            if(area.IsPlayerNearby)
                closets.Add(area);
        }
        return closets;
    }
}
