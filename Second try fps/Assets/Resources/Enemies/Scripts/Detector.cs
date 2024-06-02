using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class Detector : MonoBehaviour
{
    public bool IsPlayerInRoom { get; private set; } = false;
    public bool HasPlayerRanAway { get; private set; } = false;


    private void OnTriggerEnter(Collider other)
    {
        RoomEntrance entrance;

        if(other.gameObject.TryGetComponent<RoomEntrance>(out  entrance))
        {
            // in case player comes into the room first
            IsPlayerInRoom = entrance.PlayerEntered;
            HasPlayerRanAway = entrance.PlayerExited;

            entrance.OnPlayerEntered += () =>
            {
                IsPlayerInRoom = true;
                HasPlayerRanAway = false;
            };
            entrance.OnPlayerExited += () =>
            {
                IsPlayerInRoom = false;
                HasPlayerRanAway = true;
            };
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RoomEntrance entrance;

        if (other.gameObject.TryGetComponent<RoomEntrance>(out entrance))
        {
            entrance.OnPlayerEntered -= () =>
            {
                IsPlayerInRoom = true;
                HasPlayerRanAway = false;
            };
            entrance.OnPlayerExited -= () =>
            {
                IsPlayerInRoom = false;
                HasPlayerRanAway = true;
            };
        }
    }
}
