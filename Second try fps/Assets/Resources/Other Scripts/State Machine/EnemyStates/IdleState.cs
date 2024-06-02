using UnityEngine;
using UnityEngine.UI;

public class IdleState : IState
{
    private Vector3 stayPosition;
    private Enemy enemy;

    private Image stateImage;

    public IdleState(Enemy enemy, Vector3 stayPosition, Image image)
    {
        this.stayPosition = stayPosition;
        this.enemy = enemy;
        this.stateImage = image;
    }

    public void OnEnter()
    {
       stateImage.color = Color.gray;

        if(enemy.transform.position != stayPosition)
        {
            enemy.GoTo(stayPosition);
        }
    }

    public void OnExit()
    {
       
    }

    public void Tick()
    {
        
    }
}
