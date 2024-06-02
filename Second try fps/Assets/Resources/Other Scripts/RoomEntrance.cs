using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class RoomEntrance : MonoBehaviour
{
    public Image stateImgae;

    public event UnityAction OnPlayerEntered;
    public event UnityAction OnPlayerExited;

    public bool PlayerEntered { get; private set; } = false;
    public bool PlayerExited { get; private set; } = false;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            stateImgae.color = Color.green;
            OnPlayerEntered?.Invoke();
            PlayerEntered = true;
            PlayerExited = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            stateImgae.color = Color.red;
            OnPlayerExited?.Invoke();
            PlayerExited = true;
            PlayerEntered = false;
        }
    }
}
