namespace towers.abilities
{
    public enum AbilityTrigger
    {
        OnHit,      // After damage dealt (B, P, G)
        Passive,    // Always active (Q)
        Aura,       // Continuous proximity (E)
        OnAttack    // Before damage dealt (D, R, Y)
    }
}
