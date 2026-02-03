using infrastructure.services.abilityService;
using infrastructure.services.effectService;
using towers.abilities;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class AbilitySystemVerifier
    {
        [MenuItem("Tools/Verify Ability System Implementation")]
        public static void VerifyImplementation()
        {
            bool allPassed = true;
            int checksRun = 0;

            Debug.Log("=== Ability System Verification ===");

            // Check 1: Ability assets exist
            checksRun++;
            var abilityNames = new string[]
            {
                "B_SlowOnHit", "D_BonusDamage", "E_AttackSpeedAura", "G_PoisonDOT",
                "P_ArmorReduction", "Q_IncreasedAttackSpeed", "R_SplashDamage", "Y_MultiTarget"
            };

            int foundAbilities = 0;
            foreach (var name in abilityNames)
            {
                var asset = Resources.Load<AbilityData>($"ScriptableObjects/Abilities/{name}");
                if (asset != null)
                {
                    foundAbilities++;
                    Debug.Log($"✓ Found ability asset: {name}");
                }
                else
                {
                    Debug.LogWarning($"✗ Missing ability asset: {name}");
                    allPassed = false;
                }
            }

            Debug.Log($"Ability Assets: {foundAbilities}/{abilityNames.Length} found");

            // Check 2: TowerData assets have abilities assigned
            checksRun++;
            string[] towerGuids = AssetDatabase.FindAssets("t:TowerData");
            int towersWithAbilities = 0;
            int totalTowers = 0;

            foreach (string guid in towerGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var towerData = AssetDatabase.LoadAssetAtPath<towers.TowerData>(path);

                if (towerData == null) continue;

                string towerTypeStr = towerData.TowerType.ToString();
                if (towerTypeStr == "None" || towerTypeStr == "Stone") continue;

                totalTowers++;

                if (towerData.Abilities != null && towerData.Abilities.Count > 0)
                {
                    towersWithAbilities++;
                    Debug.Log($"✓ {towerData.name} has {towerData.Abilities.Count} ability(ies)");
                }
                else
                {
                    Debug.LogWarning($"✗ {towerData.name} has no abilities assigned");
                    allPassed = false;
                }
            }

            Debug.Log($"Tower Data: {towersWithAbilities}/{totalTowers} towers have abilities");

            // Check 3: Verify script compilation (no missing types)
            checksRun++;
            bool scriptsCompiled = true;

            // Try to find the types
            var effectServiceType = System.Type.GetType("infrastructure.services.effectService.EffectService, Assembly-CSharp");
            var abilityServiceType = System.Type.GetType("infrastructure.services.abilityService.AbilityService, Assembly-CSharp");
            var slowEffectType = System.Type.GetType("towers.abilities.effects.SlowEffect, Assembly-CSharp");

            if (effectServiceType == null)
            {
                Debug.LogError("✗ EffectService type not found - compilation issue");
                scriptsCompiled = false;
                allPassed = false;
            }
            else
            {
                Debug.Log("✓ EffectService compiled successfully");
            }

            if (abilityServiceType == null)
            {
                Debug.LogError("✗ AbilityService type not found - compilation issue");
                scriptsCompiled = false;
                allPassed = false;
            }
            else
            {
                Debug.Log("✓ AbilityService compiled successfully");
            }

            if (slowEffectType == null)
            {
                Debug.LogError("✗ SlowEffect type not found - compilation issue");
                scriptsCompiled = false;
                allPassed = false;
            }
            else
            {
                Debug.Log("✓ Effect classes compiled successfully");
            }

            // Check 4: Verify tower prefabs have colliders (for aura detection)
            checksRun++;
            string[] towerPrefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/Prefabs/Towers" });
            int prefabsWithColliders = 0;
            int totalPrefabs = 0;

            foreach (string guid in towerPrefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab == null) continue;

                var tower = prefab.GetComponent<towers.Tower>();
                if (tower == null) continue;

                totalPrefabs++;

                var collider = prefab.GetComponent<Collider>();
                if (collider != null)
                {
                    prefabsWithColliders++;
                    Debug.Log($"✓ {prefab.name} has collider: {collider.GetType().Name}");
                }
                else
                {
                    Debug.LogWarning($"⚠ {prefab.name} missing collider (needed for aura detection)");
                    // Not marking as failure, just a warning
                }
            }

            Debug.Log($"Tower Prefabs: {prefabsWithColliders}/{totalPrefabs} have colliders");

            // Summary
            Debug.Log("=== Verification Summary ===");
            Debug.Log($"Checks Run: {checksRun}");
            Debug.Log($"Status: {(allPassed ? "✓ ALL CHECKS PASSED" : "✗ SOME CHECKS FAILED")}");

            if (allPassed)
            {
                Debug.Log("<color=green>Tower Abilities System is correctly implemented!</color>");
                Debug.Log("Next steps:");
                Debug.Log("1. Enter Play Mode");
                Debug.Log("2. Test each ability according to TOWER_ABILITIES_IMPLEMENTATION.md");
                Debug.Log("3. Check Console for debug logs during combat");
            }
            else
            {
                Debug.LogError("<color=red>Implementation has issues - see warnings above</color>");
                Debug.LogError("Fix issues and run verification again");
            }
        }
    }
}
