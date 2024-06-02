using System.Collections.Generic;
using UnityEngine;

public class CoverArea : MonoBehaviour
{
    [SerializeField] private List<Transform> coverPositions;

    public override string ToString()
    {
        return gameObject.name + transform.position;
    }

    private int coversAvailable;

    private void Awake()
    {
        coversAvailable = coverPositions.Count;
    }

    public bool HasCoverAvailable() => coversAvailable > 0;

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
