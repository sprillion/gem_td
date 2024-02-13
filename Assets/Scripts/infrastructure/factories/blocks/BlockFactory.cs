using System.Collections.Generic;
using System.Linq;
using infrastructure.services.resourceProvider;
using level.builder;
using Zenject;

namespace infrastructure.factories.blocks
{
    public class BlockFactory : IBlockFactory
    {
        private const string PrefabsBlocksPath = "Prefabs/Blocks/";
        private readonly IResourceProvider _resourceProvider;
        private readonly DiContainer _diContainer;

        private List<Block> _blocks;

        private Block _darkBlock;
        private Block _lightBlock;

        public BlockFactory(IResourceProvider resourceProvider, DiContainer diContainer)
        {
            _resourceProvider = resourceProvider;
            _diContainer = diContainer;
            Load();
        }

        public Block Create(BlockType blockType)
        {
            Block prefab = GetPrefabBlock(blockType);
            if (prefab == null) return null;
            Block block = _diContainer.InstantiatePrefabForComponent<Block>(prefab);
            return block;
        }

        private Block GetPrefabBlock(BlockType blockType)
        {
            return _blocks.FirstOrDefault(block => block.BlockType == blockType);
        }
        
        private void Load()
        {
            _blocks = _resourceProvider.LoadList<Block>(PrefabsBlocksPath);
        }
    }
}
