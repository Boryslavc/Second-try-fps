using UnityEngine;
using UnityEngine.UI;

public class BlueCombat : IState
{
    private BlueEnemy blue;

    private Transform player;

    private CoverArea area;
    private Transform cover;

    private StateMachine subStateMachine;
    private RunToCoverState runToCover;
    private ShootState shoot;
    private ReloadState reload;

    private Image image; 


    public BlueCombat(BlueEnemy blue, Image image)
    {
        this.blue = blue;
        this.image = image;

        player = GameObject.FindWithTag("Player").transform;
    }

    public void OnEnter()
    {

        runToCover = new RunToCoverState(blue, image);
        shoot = new ShootState(blue, image);
        reload = new ReloadState(blue, image);

        subStateMachine = new StateMachine();

        subStateMachine.AddAnyTransition(runToCover, () => !runToCover.CloseEnoughToPlayer);
        subStateMachine.AddTransition(runToCover, shoot, () => runToCover.CloseEnoughToPlayer);
        subStateMachine.AddTransition(shoot, reload, blue.IsOutOfAmmo);
        subStateMachine.AddTransition(reload, shoot, reload.DoneReloading);
    }



    public void Tick()
    {
        subStateMachine?.Tick();
    }

    public void OnExit()
    {
        area.GetCoverBack(cover);
    }
}
