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
        private ILevelBuilder _levelBuilder;
        private Tower _currentTower;
        public BlockType BlockType => _blockType;
        
        public int X { get; private set; }
        public int Y { get; private set; }
        public float Size { get; private set; }

        [Inject]
        public void Construct(ILevelBuilder levelBuilder)
        {
            _levelBuilder = levelBuilder;
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
            _currentTower = _levelBuilder.CreateTower(X, Y);
            Debug.Log($"{X}   {Y}");
        }
    }
}
