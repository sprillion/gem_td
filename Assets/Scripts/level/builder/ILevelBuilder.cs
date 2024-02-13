namespace level.builder
{
    public interface ILevelBuilder
    {
        MapData MapData { get; }
        void Build();
    }
}
