using UnityEngine;
using UnityEngine.UI;

public class ReloadState : IState
{
    private BlueEnemy blue;

    private Image image;

    public ReloadState (BlueEnemy blue, Image image)
    {
        this.blue = blue;
        this.image = image;
    }

    public void OnEnter()
    {
        image.color = Color.blue;
    }


    public void Tick()
    {

    }

    public bool DoneReloading()
    {
        //checking for value occurs every 0.2 sec, so need low probability 
        int value = Random.Range(0, 2000);
        if (value < 5)
        {
            blue.Reload();
            return true;
        }
        return false;
    }

    public void OnExit()
    {

    }
}
