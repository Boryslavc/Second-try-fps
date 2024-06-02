using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Info", menuName = "GunSO/Info" )]
public class WeaponConfig : ScriptableObject
{
    public float Damage;
    public float Distance;
    public float TimeBetweenShots;
    public bool IsSpreadApplied;
    public SpreadTypeEnum SpreadType;
    public Vector3 Spread;
    public float RecoilRecoverySpeed;
    public float MaxSpreadTime;
    public int MagazineSize;

    public Vector3 GetSpread(float shootingTime = 0)
    {
        if(SpreadType == SpreadTypeEnum.None)
        {
            return Vector3.zero;
        }
        else if(SpreadType == SpreadTypeEnum.Simple)
        {
            Vector3 spread = Vector3.Lerp(
                Vector3.zero,
                new Vector3(Random.Range(-Spread.x, Spread.x),
                Random.Range(-Spread.y, Spread.y),
                Random.Range(-Spread.z, Spread.z)),
                Mathf.Clamp01(shootingTime / MaxSpreadTime));
            return spread;
        }
        return Vector3.zero;
    }
}
