using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public abstract void SetRayCastOrigin(Transform point);
    public abstract void AddAmmo();
    public abstract void Tick(bool shootIntention);
}
