namespace towers
{
    public static class TowerTypeExtensions
    {
        public static bool IsBasicTower(this TowerType towerType)
        {
            switch (towerType)
            {
                case TowerType.P:
                case TowerType.Q:
                case TowerType.D:
                case TowerType.G:
                case TowerType.E:
                case TowerType.R:
                case TowerType.B:
                case TowerType.Y:
                    return true;
                default:
                    return false;
            }
        }
    }
}
