using UnityEngine;

namespace towers
{
    [CreateAssetMenu(fileName = "Tower", menuName = "Data/Tower", order = 0)]
    public class TowerData : ScriptableObject
    {
        public TowerType TowerType;
        public int Level = 1;
        public int Damage;
        public int AttackSpeed;
        public float AttackRange;
    }
}