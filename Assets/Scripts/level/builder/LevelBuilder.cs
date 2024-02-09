using infrastructure.factories.blocks;
using infrastructure.services.resourceProvider;
using UnityEngine;

namespace level.builder
{
    public class LevelBuilder : ILevelBuilder
    {
        private const string MapPath = "ScriptableObjects/Maps/Map";
        
        private readonly IResourceProvider _resourceProvider;
        private readonly IBlockFactory _blockFactory;
        private MapData _mapData;

        public LevelBuilder(IResourceProvider resourceProvider, IBlockFactory blockFactory)
        {
            _resourceProvider = resourceProvider;
            _blockFactory = blockFactory;
            Load();
            Build();
        }
        
        public void Build()
        {
            var parent = new GameObject("Map").transform;
            
            for (int i = 0; i < _mapData.Width; i++)
            {
                for (int j = 0; j < _mapData.Height; j++)
                {
                    CreateBlock(i, j, _mapData.BlocksMap[i, j], parent);        
                }
            }
        }

        private void Load()
        {
            _mapData = _resourceProvider.Load<MapData>(MapPath);
        }

        private void CreateBlock(int x, int y, BlockType blockType, Transform parent)
        {
            var block = _blockFactory.Create(blockType);
            if (block == null) return;
            block.transform.position = new Vector3(x * _mapData.BlockSize, 0, -y * _mapData.BlockSize);
            block.transform.SetParent(parent);
            block.SetPosition(x, y, _mapData.BlockSize);
        }
    }
}
