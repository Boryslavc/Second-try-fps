using System.Collections.Generic;
using UnityEngine;

public class AIPAthfinder
{
    private Enemy enemy;
    private float nearestCoverSearchRadius;

    private LayerMask layerMask;

    public AIPAthfinder(Enemy enemy)
    {
        this.enemy = enemy;
        layerMask = LayerMask.GetMask("Hideable");
    }

    public List<Transform> FindPathTo(Vector3 target)
    {
        Collider[] colliders1 = Physics.OverlapSphere(target, nearestCoverSearchRadius,
            layerMask);
        Transform closestCoverToTarget = SearchNearestCover(colliders1);

        Collider[] colliders = Physics.OverlapSphere(enemy.transform.position,
            nearestCoverSearchRadius, layerMask);

        if (colliders.Length > 0)
        {
            Transform nearest = SearchNearestCover(colliders);

            List<Transform> path = AStar(nearest, closestCoverToTarget);
            return path;
        }
        else
        {
            Debug.Log($"No Hideable spoits were found around {enemy.transform.position}");
            return null;
        }
    }

    private Transform SearchNearestCover(Collider[] colliders)
    {
        float nearestDist = Vector3.Distance(colliders[0].transform.position,
            enemy.transform.position );
        int ind = 0;

        for(int i = 1; i < colliders.Length; i++)
        {
            if (Vector3.Distance(enemy.transform.position, colliders[i].transform.position)
                < nearestDist)
            {
                nearestDist = Vector3.Distance(enemy.transform.position, colliders[i].transform.position);
                ind = i;
            }
        }

        return colliders[ind].transform;
    }

    private List<Transform> AStar(Transform startPoint, Transform endPoint)
    {
        List<Transform> path = new List<Transform>();

        List<CoverArea> redAreas = new List<CoverArea>();
        List<CoverArea> greenAreas = new List<CoverArea>();

        CoverArea startArea = startPoint.gameObject.GetComponent<CoverArea>();
        CoverArea endArea = endPoint.gameObject.GetComponent<CoverArea>();

        foreach(CoverArea coverArea in startArea.incidentAreas)
        {
            coverArea.sqrDistToGoal = Vector3.SqrMagnitude(coverArea.transform.position
                - endPoint.transform.position);
            coverArea.sqrDistFromStart = Vector3.SqrMagnitude(coverArea.transform.position
                - startPoint.position);

            coverArea.cameFrom = startArea;
            greenAreas.Add(coverArea);
        }

        while(greenAreas.Count > 0)
        {
            CoverArea next = FindLowestFValue(greenAreas);

            if(next == endArea)
            {
                break;
            }
            else
            {
                foreach(CoverArea coverArea in next.incidentAreas)
                {
                    if(!redAreas.Contains(coverArea))
                    {
                        float toGoal = Vector3.SqrMagnitude(coverArea.transform.position
                            - endArea.transform.position);
                        float fromStart = Vector3.SqrMagnitude(coverArea.transform.position
                            - endArea.transform.position);

                        if(toGoal + fromStart < coverArea.sqrDistFromStart + coverArea.sqrDistToGoal
                            && coverArea.cameFrom != null)
                        {
                            coverArea.sqrDistFromStart = fromStart;
                            coverArea.sqrDistToGoal = toGoal;
                            coverArea.cameFrom = next;
                        }
                       
                        greenAreas.Add(coverArea);
                    }
                }
            }
            redAreas.Add(next);
        }

        CoverArea area = endArea;

        while(area.cameFrom != null)
        {
            path.Add(area.transform);

            area = area.cameFrom;
        }
        path.Reverse();

        ClearAreas(greenAreas, redAreas);
        return path;
    }

    private CoverArea FindLowestFValue(List<CoverArea> areas)
    {
        float lowest = areas[0].sqrDistToGoal + areas[0].sqrDistFromStart;
        int ind = 0;

        for(int i = 1; i < areas.Count; i++)
        {
            float f = areas[i].sqrDistToGoal + areas[i].sqrDistFromStart;

            if(f < lowest)
            {
                lowest = f;
                ind = i;
            }
        }
        return areas[ind];
    }

    private void ClearAreas(List<CoverArea> red, List<CoverArea> green) 
    {
        foreach(CoverArea area in red)
        {
            area.cameFrom = null;
            area.sqrDistFromStart = 0;
            area.sqrDistToGoal = 0;
        }

        foreach(CoverArea area in green)
        {
            area.cameFrom = null;
            area.sqrDistFromStart = 0;
            area.sqrDistToGoal = 0;
        }
    }
}
