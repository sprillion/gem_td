using towers;

namespace level.builder
{
    public interface ILevelBuilder
    {
        MapData MapData { get; }
        void Build();
        Tower CreateTower(int x, int y);
    }
}
