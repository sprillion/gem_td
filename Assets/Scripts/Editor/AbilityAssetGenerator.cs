using towers.abilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AbilityAssetGenerator
    {
        [MenuItem("Tools/Generate All Ability Assets")]
        public static void GenerateAllAbilityAssets()
        {
            string basePath = "Assets/Resources/ScriptableObjects/Abilities/";

            // Create B_SlowOnHit
            var slowOnHit = ScriptableObject.CreateInstance<SlowOnHitAbility>();
            slowOnHit.AbilityType = AbilityType.SlowOnHit;
            slowOnHit.TriggerType = AbilityTrigger.OnHit;
            slowOnHit.Description = "Slows enemy movement speed on hit";
            slowOnHit.SlowPercentByLevel = new float[] { 20f, 30f, 40f, 50f };
            slowOnHit.DurationByLevel = new float[] { 2f, 2.5f, 3f, 3.5f };
            AssetDatabase.CreateAsset(slowOnHit, basePath + "B_SlowOnHit.asset");

            // Create D_BonusDamage
            var bonusDamage = ScriptableObject.CreateInstance<BonusDamageAbility>();
            bonusDamage.AbilityType = AbilityType.BonusDamage;
            bonusDamage.TriggerType = AbilityTrigger.OnAttack;
            bonusDamage.Description = "Deals bonus damage on attack";
            bonusDamage.BonusDamageByLevel = new int[] { 5, 10, 15, 20 };
            AssetDatabase.CreateAsset(bonusDamage, basePath + "D_BonusDamage.asset");

            // Create E_AttackSpeedAura
            var attackSpeedAura = ScriptableObject.CreateInstance<AttackSpeedAuraAbility>();
            attackSpeedAura.AbilityType = AbilityType.AttackSpeedAura;
            attackSpeedAura.TriggerType = AbilityTrigger.Aura;
            attackSpeedAura.Description = "Increases attack speed of nearby towers";
            attackSpeedAura.AuraRadiusByLevel = new float[] { 5f, 6f, 7f, 8f };
            attackSpeedAura.AttackSpeedMultiplierByLevel = new float[] { 0.1f, 0.15f, 0.2f, 0.25f };
            AssetDatabase.CreateAsset(attackSpeedAura, basePath + "E_AttackSpeedAura.asset");

            // Create G_PoisonDOT
            var poisonDOT = ScriptableObject.CreateInstance<PoisonDOTAbility>();
            poisonDOT.AbilityType = AbilityType.PoisonDOT;
            poisonDOT.TriggerType = AbilityTrigger.OnHit;
            poisonDOT.Description = "Applies poison damage over time";
            poisonDOT.DamagePerTickByLevel = new int[] { 1, 2, 3, 5 };
            poisonDOT.TickIntervalByLevel = new float[] { 0.5f, 0.5f, 0.5f, 0.5f };
            poisonDOT.DurationByLevel = new float[] { 3f, 4f, 5f, 6f };
            AssetDatabase.CreateAsset(poisonDOT, basePath + "G_PoisonDOT.asset");

            // Create P_ArmorReduction
            var armorReduction = ScriptableObject.CreateInstance<ArmorReductionAbility>();
            armorReduction.AbilityType = AbilityType.ArmorReduction;
            armorReduction.TriggerType = AbilityTrigger.OnHit;
            armorReduction.Description = "Reduces enemy armor temporarily";
            armorReduction.ArmorReductionByLevel = new int[] { 5, 10, 15, 20 };
            armorReduction.DurationByLevel = new float[] { 3f, 3.5f, 4f, 4.5f };
            AssetDatabase.CreateAsset(armorReduction, basePath + "P_ArmorReduction.asset");

            // Create Q_IncreasedAttackSpeed
            var increasedAttackSpeed = ScriptableObject.CreateInstance<IncreasedAttackSpeedAbility>();
            increasedAttackSpeed.AbilityType = AbilityType.IncreasedAttackSpeed;
            increasedAttackSpeed.TriggerType = AbilityTrigger.Passive;
            increasedAttackSpeed.Description = "Passively increases own attack speed";
            increasedAttackSpeed.AttackSpeedMultiplierByLevel = new float[] { 0.15f, 0.2f, 0.25f, 0.3f };
            AssetDatabase.CreateAsset(increasedAttackSpeed, basePath + "Q_IncreasedAttackSpeed.asset");

            // Create R_SplashDamage
            var splashDamage = ScriptableObject.CreateInstance<SplashDamageAbility>();
            splashDamage.AbilityType = AbilityType.SplashDamage;
            splashDamage.TriggerType = AbilityTrigger.OnAttack;
            splashDamage.Description = "Damages enemies near the primary target";
            splashDamage.SplashRadiusByLevel = new float[] { 3f, 3.5f, 4f, 4.5f };
            splashDamage.SplashDamagePercentByLevel = new float[] { 50f, 60f, 70f, 80f };
            AssetDatabase.CreateAsset(splashDamage, basePath + "R_SplashDamage.asset");

            // Create Y_MultiTarget
            var multiTarget = ScriptableObject.CreateInstance<MultiTargetAbility>();
            multiTarget.AbilityType = AbilityType.MultiTarget;
            multiTarget.TriggerType = AbilityTrigger.OnAttack;
            multiTarget.Description = "Attacks multiple enemies simultaneously";
            multiTarget.AdditionalTargetsByLevel = new int[] { 1, 2, 3, 4 };
            AssetDatabase.CreateAsset(multiTarget, basePath + "Y_MultiTarget.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Successfully generated all 8 ability assets in: " + basePath);
        }
    }
}
