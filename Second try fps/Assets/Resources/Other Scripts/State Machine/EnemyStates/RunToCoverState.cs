using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RunToCoverState : IState
{
    private BlueEnemy blue;
    private Transform player;

    private Transform cover;
    private Vector3 position;

    private Image stateImage;

    public RunToCoverState(BlueEnemy blue, Transform cover, Image image)
    {
        this.blue = blue;
        this.cover = cover;
        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;

        stateImage = image;
    }


    public bool HasArrived
    {
        get
        {
            return blue.transform.position.HorizontalSquaredDistanceTo(position) < 2f;
        }
    }

    public void OnEnter()
    {
        ComputePosition();

        blue.SpeedUpBy(2f);
        blue.GoTo(position);

        stateImage.color = Color.green;
    }
    private void ComputePosition()
    {
        var dirToPlayer = (player.position - blue.transform.position).normalized;
        if (Vector3.Dot(blue.transform.forward, dirToPlayer) > 0)
        {
            position = cover.position - cover.forward * 2f;
        }
        else
        {
            position = cover.position + cover.forward * 2f;
        }
        Debug.DrawRay(position, Vector3.up * 5f, Color.black, 2f);
    }

    public void Tick()
    {

    }

    public void OnExit()
    {
        blue.SpeedUpBy(-2f);
    }  
}
