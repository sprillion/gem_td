using infrastructure.factories.blocks;
using UnityEngine;

namespace level.builder
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private BlockType _blockType;
        public BlockType BlockType => _blockType;
        
        public int X { get; private set; }
        public int Y { get; private set; }
        public float Size { get; private set; }

        public void SetPosition(int x, int y, float size)
        {
            X = x;
            Y = y;
            Size = size;
        }
    }
}
