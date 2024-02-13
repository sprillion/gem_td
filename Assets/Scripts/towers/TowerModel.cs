using UnityEngine;

namespace towers
{
    public class TowerModel : MonoBehaviour
    {
        [SerializeField] private TowerType _towerType;
        [SerializeField] private Animator _animator;

        public TowerType TowerType => _towerType;
    }
}