using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfo",menuName ="Enemies/Enemy Info")]
public class EnemyData : ScriptableObject
{
    public float AttackDistance;
    public float Damage;
    public float AttackSpeed;
    public float Speed;
}
