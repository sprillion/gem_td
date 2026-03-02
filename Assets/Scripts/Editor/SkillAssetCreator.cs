#if UNITY_EDITOR
using infrastructure.services.gameStateService;
using skills;
using skills.data;
using towers;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class SkillAssetCreator
    {
        private const string BasePath = "Assets/Resources/ScriptableObjects/Skills/";

        [MenuItem("Tools/Create Skill Assets")]
        public static void CreateAllSkillAssets()
        {
            CreateSwapTowers();
            CreateRestoreHealth();
            CreateTowerBuffs();
            CreateTowerTypeChances();
            CreateTowerLevelChances();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("All 16 skill assets created successfully!");
        }

        private static void CreateSwapTowers()
        {
            var asset = ScriptableObject.CreateInstance<SwapTowersSkillData>();
            asset.SkillType = PlayerSkillType.SwapTowers;
            asset.SkillName = "Swap Towers";
            asset.Description = "Swap positions of two towers on the map";
            asset.TargetMode = SkillTargetMode.TwoTowers;
            asset.AvailableInPhases = new[] { GamePhase.PLACING_TOWERS, GamePhase.COMBAT };
            asset.GoldCostByLevel = new[] { 50, 40, 30, 20 };
            asset.CooldownByLevel = new[] { 30f, 25f, 20f, 15f };
            SaveAsset(asset, "SwapTowers");
        }

        private static void CreateRestoreHealth()
        {
            var asset = ScriptableObject.CreateInstance<RestoreHealthSkillData>();
            asset.SkillType = PlayerSkillType.RestoreHealth;
            asset.SkillName = "Restore Health";
            asset.Description = "Restore a random amount of HP";
            asset.TargetMode = SkillTargetMode.None;
            asset.AvailableInPhases = new[] { GamePhase.PLACING_TOWERS, GamePhase.COMBAT, GamePhase.SELECTING_TOWER };
            asset.GoldCostByLevel = new[] { 100, 80, 60, 40 };
            asset.CooldownByLevel = new[] { 60f, 50f, 40f, 30f };
            asset.MinHealByLevel = new[] { 1, 2, 3, 5 };
            asset.MaxHealByLevel = new[] { 3, 5, 8, 10 };
            SaveAsset(asset, "RestoreHealth");
        }

        private static void CreateTowerBuffs()
        {
            // IncreaseRange
            var range = ScriptableObject.CreateInstance<TowerBuffSkillData>();
            range.SkillType = PlayerSkillType.IncreaseRange;
            range.SkillName = "Increase Range";
            range.Description = "Temporarily increase a tower's attack range";
            range.TargetMode = SkillTargetMode.SingleTower;
            range.AvailableInPhases = new[] { GamePhase.COMBAT };
            range.GoldCostByLevel = new[] { 30, 25, 20, 15 };
            range.CooldownByLevel = new[] { 20f, 18f, 15f, 12f };
            range.BuffValueByLevel = new[] { 1f, 1.5f, 2f, 3f };
            range.BuffDurationByLevel = new[] { 10f, 12f, 15f, 20f };
            SaveAsset(range, "IncreaseRange");

            // IncreaseAttackSpeed
            var speed = ScriptableObject.CreateInstance<TowerBuffSkillData>();
            speed.SkillType = PlayerSkillType.IncreaseAttackSpeed;
            speed.SkillName = "Increase Attack Speed";
            speed.Description = "Temporarily speed up a tower's attacks";
            speed.TargetMode = SkillTargetMode.SingleTower;
            speed.AvailableInPhases = new[] { GamePhase.COMBAT };
            speed.GoldCostByLevel = new[] { 30, 25, 20, 15 };
            speed.CooldownByLevel = new[] { 20f, 18f, 15f, 12f };
            speed.BuffValueByLevel = new[] { 0.25f, 0.5f, 0.75f, 1f };
            speed.BuffDurationByLevel = new[] { 10f, 12f, 15f, 20f };
            SaveAsset(speed, "IncreaseAttackSpeed");

            // IncreaseDamage
            var damage = ScriptableObject.CreateInstance<TowerBuffSkillData>();
            damage.SkillType = PlayerSkillType.IncreaseDamage;
            damage.SkillName = "Increase Damage";
            damage.Description = "Temporarily increase a tower's damage";
            damage.TargetMode = SkillTargetMode.SingleTower;
            damage.AvailableInPhases = new[] { GamePhase.COMBAT };
            damage.GoldCostByLevel = new[] { 30, 25, 20, 15 };
            damage.CooldownByLevel = new[] { 20f, 18f, 15f, 12f };
            damage.BuffValueByLevel = new[] { 5f, 10f, 20f, 35f };
            damage.BuffDurationByLevel = new[] { 10f, 12f, 15f, 20f };
            SaveAsset(damage, "IncreaseDamage");
        }

        private static void CreateTowerTypeChances()
        {
            var types = new[]
            {
                (TowerType.P, PlayerSkillType.ChanceP, "P"),
                (TowerType.Q, PlayerSkillType.ChanceQ, "Q"),
                (TowerType.D, PlayerSkillType.ChanceD, "D"),
                (TowerType.G, PlayerSkillType.ChanceG, "G"),
                (TowerType.E, PlayerSkillType.ChanceE, "E"),
                (TowerType.R, PlayerSkillType.ChanceR, "R"),
                (TowerType.B, PlayerSkillType.ChanceB, "B"),
                (TowerType.Y, PlayerSkillType.ChanceY, "Y"),
            };

            foreach (var (towerType, skillType, letter) in types)
            {
                var asset = ScriptableObject.CreateInstance<TowerTypeChanceSkillData>();
                asset.SkillType = skillType;
                asset.SkillName = $"Chance {letter}";
                asset.Description = $"Increase chance of next tower being type {letter}";
                asset.TargetMode = SkillTargetMode.None;
                asset.AvailableInPhases = new[] { GamePhase.PLACING_TOWERS };
                asset.GoldCostByLevel = new[] { 20, 15, 10, 5 };
                asset.CooldownByLevel = new[] { 10f, 8f, 6f, 4f };
                asset.TargetTowerType = towerType;
                asset.BonusChanceByLevel = new[] { 30f, 50f, 70f, 90f };
                SaveAsset(asset, $"Chance{letter}");
            }
        }

        private static void CreateTowerLevelChances()
        {
            for (int level = 1; level <= 3; level++)
            {
                var asset = ScriptableObject.CreateInstance<TowerLevelChanceSkillData>();
                asset.SkillType = (PlayerSkillType)(29 + level); // ChanceLevel1=30, ChanceLevel2=31, ChanceLevel3=32
                asset.SkillName = $"Chance Level {level}";
                asset.Description = $"Increase chance of next tower being level {level}";
                asset.TargetMode = SkillTargetMode.None;
                asset.AvailableInPhases = new[] { GamePhase.PLACING_TOWERS };
                asset.GoldCostByLevel = new[] { 25 * level, 20 * level, 15 * level, 10 * level };
                asset.CooldownByLevel = new[] { 10f, 8f, 6f, 4f };
                asset.TargetTowerLevel = level;
                asset.BonusChanceByLevel = new[] { 30f, 50f, 70f, 90f };
                SaveAsset(asset, $"ChanceLevel{level}");
            }
        }

        private static void SaveAsset(ScriptableObject asset, string name)
        {
            string path = BasePath + name + ".asset";
            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created skill asset: {path}");
        }
    }
}
#endif
