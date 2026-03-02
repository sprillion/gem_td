namespace skills
{
    public class PlayerSkillInstance
    {
        public PlayerSkillData Data { get; private set; }
        public int UpgradeLevel { get; private set; }
        public float CooldownRemaining { get; set; }

        public int GoldCost => Data.GoldCostByLevel[UpgradeLevel];
        public float Cooldown => Data.CooldownByLevel[UpgradeLevel];
        public bool IsOnCooldown => CooldownRemaining > 0f;

        public PlayerSkillInstance(PlayerSkillData data, int upgradeLevel)
        {
            Data = data;
            UpgradeLevel = upgradeLevel;
            CooldownRemaining = 0f;
        }
    }
}
