using UnityEngine;
using UnityEngine.UI;

public class RedCombat : IState
{
    private Transform player;
    private RedEnemy red;
    private Image stateImage;

    private Vector3 destination;
    private float attackDistanceSquared;
    private float rotationSpeed = 100f;

    public RedCombat(RedEnemy red, float attackDistance, Image image)
    {
        this.red = red;
        this.stateImage = image;

        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;

        attackDistanceSquared = attackDistance * attackDistance;
    }

    public void OnEnter()
    {
        stateImage.color = Color.red;
    }

    public void Tick()
    {
        if(InAttackRange())
        {
            red.Attack();
        }
        else if(PlayerMovedSignificantly())
        {
            var offset = (player.transform.position - red.transform.position).normalized;
            destination = player.transform.position + offset;
            destination.y -= 1f;
            red.GoTo(destination);
        }
        LookAtPlayer();
    }
    private bool InAttackRange()
    {
        // yes, even if player is above the enemy. Prevents player from jumping down.
        return red.transform.position.HorizontalSquaredDistanceTo(player.transform.position)
            < attackDistanceSquared;
    }

    private bool PlayerMovedSignificantly()
    {
        return player.position.HorizontalSquaredDistanceTo(destination) > attackDistanceSquared;
    }
    private void LookAtPlayer()
    {
        Vector3 lookDirection = (player.position - red.transform.position).normalized;
        if (lookDirection != Vector3.zero && red.transform.forward != lookDirection)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            red.transform.rotation =
                Quaternion.RotateTowards(red.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    public void OnExit()
    {

    }
}
