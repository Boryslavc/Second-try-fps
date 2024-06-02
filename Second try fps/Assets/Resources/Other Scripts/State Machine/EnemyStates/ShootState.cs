using UnityEngine;
using UnityEngine.UI;

public class ShootState : IState
{
    private Transform player;
    private BlueEnemy blue;

    private Image stateImage;

    private float FOV = 360f;
    private Vector3 lastPlayerPosition;
    private float rotationSpeed = 100f;
    private LayerMask ignoreLayer;

    private Vector3 eyesLevel;

    public ShootState(BlueEnemy blue, Image image)
    {
        this.stateImage = image;
        this.blue = blue;

        player = GameObject.FindAnyObjectByType<PlayerMovement>().transform;

        ignoreLayer = LayerMask.GetMask("Enemy");
    }

    public void OnEnter()
    {
        stateImage.color = Color.red;
        lastPlayerPosition = player.position;

        var rb = blue.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition;
    }

    public void Tick()
    {
        if(PlayerInSight())
        {
            blue.Shoot();
        }
        LookAtPlayer();
    }
    private bool PlayerInSight()
    {
        var direction = (player.position - blue.transform.position).normalized;

        if(Vector3.Dot(direction, blue.transform.forward) >= Mathf.Cos(FOV))
        {
            eyesLevel = blue.transform.position + new Vector3(0, 0.5f, 0);
            RaycastHit hit;
            if (Physics.Raycast(eyesLevel, direction, out hit, 100f, ~ignoreLayer))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    lastPlayerPosition = hit.transform.position;
                    return true;
                }
            }
        }
        return false;
    }
    private void LookAtPlayer()
    {
        Vector3 lookDirection = (lastPlayerPosition - blue.transform.position).normalized;
        if (lookDirection != Vector3.zero && blue.transform.forward != lookDirection)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            blue.transform.rotation =
                Quaternion.RotateTowards(blue.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void OnExit()
    {
        var rb = blue.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.None;
    }
}
