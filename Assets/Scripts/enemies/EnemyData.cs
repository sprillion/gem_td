using UnityEngine;

namespace enemies
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy", order = 2)]
    public class EnemyData : ScriptableObject
    {
        public int Health;
        public int MoveSpeed;
        public int RotateSpeed;
        public int Damage;
        public int Armor;
        public int MagicResist;
        public int Evasion;
        public EnemyMoveType EnemyMoveType;
    }
}