using infrastructure.factories.blocks;
using infrastructure.services.inputService;
using infrastructure.services.towerService;
using towers;
using UnityEngine;
using Zenject;

namespace level.builder
{
    public class Block : MonoBehaviour, IClickable
    {
        [SerializeField] private BlockType _blockType;
        private ITowerService _towerService;
        private Tower _currentTower;
        public BlockType BlockType => _blockType;
        
        public int X { get; private set; }
        public int Y { get; private set; }
        public float Size { get; private set; }

        [Inject]
        public void Construct(ITowerService towerService)
        {
            _towerService = towerService;
        }

        public void SetPosition(int x, int y, float size)
        {
            X = x;
            Y = y;
            Size = size;
        }

        public void OnClick()
        {
            if (_currentTower != null) return;
            _currentTower = _towerService.SetTower(X, Y);
            Debug.Log($"{X}   {Y}");
        }
    }
}
