using enemies;

namespace towers.projectiles
{
    public struct ProjectileTarget
    {
        public Enemy Target;
        public int Damage;
        public bool IsPrimary;

        public ProjectileTarget(Enemy target, int damage, bool isPrimary)
        {
            Target = target;
            Damage = damage;
            IsPrimary = isPrimary;
        }
    }
}
