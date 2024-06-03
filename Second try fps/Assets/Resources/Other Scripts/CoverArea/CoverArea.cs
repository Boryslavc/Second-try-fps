using System.Collections.Generic;
using UnityEngine;

public class CoverArea : MonoBehaviour
{
    [SerializeField] private List<Transform> coverPositions;
    public List<CoverArea> incidentAreas;

    public Dictionary<CoverArea, float> sqrDistToArea = new Dictionary<CoverArea, float>();

    public float sqrDistToGoal;
    public float sqrDistFromStart;

    public CoverArea cameFrom;

    public bool IsPlayerNearby { get; private set; }

    public override string ToString()
    {
        return gameObject.name + transform.position;
    }

    private int coversAvailable;

    private void Awake()
    {
        coversAvailable = coverPositions.Count;

        foreach(CoverArea coverArea in incidentAreas)
        {
            float distance = Vector3.SqrMagnitude(this.transform.position
                - coverArea.transform.position);
            sqrDistToArea.Add(coverArea, distance);
        }
    }

    public bool HasCoverAvailable() => coversAvailable > 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            IsPlayerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            IsPlayerNearby = false;
        }
    }

    public Transform ProvideRandomCover()
    {
        int randomInt = Random.Range(0, coverPositions.Count);
        var cover = coverPositions[randomInt];

        coversAvailable--;
        cover.gameObject.SetActive(false);
        return cover;
    }

    public Transform ProvideCoverPositionRelativeTo(Transform point)
    {
        var furthest = float.NegativeInfinity;

        Transform cover = null; 

        for(int i = 0; i < coverPositions.Count; i++)
        {
            // TO REFACTOR - administrative rule that we need to check for availability,
            // before getting cover
            if (!coverPositions[i].gameObject.activeSelf)
                continue;

            float distance = Vector3.Distance(point.position, coverPositions[i].position);
            if (distance > furthest)
               cover = coverPositions[i];              
        }

        cover.gameObject.SetActive(false);
        coversAvailable--;
        return cover;
    }

    public void GetCoverBack(Transform cover)
    {
        cover.gameObject.SetActive(true);
        coversAvailable++;
    }
}
