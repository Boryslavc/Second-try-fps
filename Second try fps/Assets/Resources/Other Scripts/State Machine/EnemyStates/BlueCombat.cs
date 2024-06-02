using UnityEngine;
using UnityEngine.UI;

public class BlueCombat : IState
{
    private BlueEnemy blue;

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
    }

    public void OnEnter()
    {
        FindCover();

        runToCover = new RunToCoverState(blue, cover, image);
        shoot = new ShootState(blue, image);
        reload = new ReloadState(blue, image);

        subStateMachine = new StateMachine();

        subStateMachine.AddAnyTransition(runToCover, () => !runToCover.HasArrived);
        subStateMachine.AddTransition(runToCover, shoot, () => runToCover.HasArrived);
        subStateMachine.AddTransition(shoot, reload, blue.IsOutOfAmmo);
        subStateMachine.AddTransition(reload, shoot, reload.DoneReloading);
    }
    private void FindCover()
    {
        area = LevelGeometry.Instance.GetClosestCoverAreaRelativeTo(blue.transform.position);
        cover = area.ProvideCoverPositionRelativeTo(blue.transform);
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
