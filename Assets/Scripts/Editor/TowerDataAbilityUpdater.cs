using System.Collections.Generic;
using towers;
using towers.abilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class TowerDataAbilityUpdater
    {
        [MenuItem("Tools/Update All TowerData with Abilities")]
        public static void UpdateAllTowerDataWithAbilities()
        {
            // Load all ability assets
            var abilities = new Dictionary<string, AbilityData>
            {
                { "B", Resources.Load<AbilityData>("ScriptableObjects/Abilities/B_SlowOnHit") },
                { "D", Resources.Load<AbilityData>("ScriptableObjects/Abilities/D_BonusDamage") },
                { "E", Resources.Load<AbilityData>("ScriptableObjects/Abilities/E_AttackSpeedAura") },
                { "G", Resources.Load<AbilityData>("ScriptableObjects/Abilities/G_PoisonDOT") },
                { "P", Resources.Load<AbilityData>("ScriptableObjects/Abilities/P_ArmorReduction") },
                { "Q", Resources.Load<AbilityData>("ScriptableObjects/Abilities/Q_IncreasedAttackSpeed") },
                { "R", Resources.Load<AbilityData>("ScriptableObjects/Abilities/R_SplashDamage") },
                { "Y", Resources.Load<AbilityData>("ScriptableObjects/Abilities/Y_MultiTarget") }
            };

            int updatedCount = 0;

            // Find all TowerData assets
            string[] guids = AssetDatabase.FindAssets("t:TowerData");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TowerData towerData = AssetDatabase.LoadAssetAtPath<TowerData>(path);

                if (towerData == null) continue;

                // Get tower type as string
                string towerTypeStr = towerData.TowerType.ToString();

                // Skip None and Stone types
                if (towerTypeStr == "None" || towerTypeStr == "Stone")
                    continue;

                // Check if this tower type has an ability
                if (abilities.ContainsKey(towerTypeStr) && abilities[towerTypeStr] != null)
                {
                    // Clear existing abilities and add the correct one
                    towerData.Abilities = new List<AbilityData> { abilities[towerTypeStr] };

                    EditorUtility.SetDirty(towerData);
                    updatedCount++;

                    Debug.Log($"Updated {towerData.name} ({towerTypeStr}{towerData.Level}) with {towerTypeStr}_{abilities[towerTypeStr].AbilityType} ability");
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Successfully updated {updatedCount} TowerData assets with abilities!");
        }
    }
}
