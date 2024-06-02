using UnityEngine;
using UnityEngine.Events;

public class ShootableButton : MonoBehaviour, IShootable
{
    public event UnityAction OnBeingShot;
    public void TakeBullet(float damage)
    {
        OnBeingShot?.Invoke();
    }
}
