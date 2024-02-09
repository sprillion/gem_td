using level.builder;

namespace infrastructure.factories.blocks
{
    public interface IBlockFactory
    {
        Block Create(BlockType blockType);

    }
}
