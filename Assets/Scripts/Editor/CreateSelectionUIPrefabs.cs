using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Editor
{
    public static class CreateSelectionUIPrefabs
    {
        private const string PrefabDir = "Assets/Resources/Prefabs/UI";

        [MenuItem("Tools/Create Selection UI Prefabs")]
        public static void CreatePrefabs()
        {
            EnsureDirectoryExists(PrefabDir);

            var abilityPrefab = CreateAbilityItemPrefab();
            var effectPrefab = CreateEffectItemPrefab();
            CreateSelectionInfoPanelPrefab(abilityPrefab, effectPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("All Selection UI Prefabs created successfully!");
        }

        // ───────────────────────── AbilityItem ─────────────────────────
        private static GameObject CreateAbilityItemPrefab()
        {
            var root = new GameObject("AbilityItem");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(350, 60);

            var layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var le = root.AddComponent<LayoutElement>();
            le.preferredHeight = 60;

            // Icon
            var iconGO = CreateUIObject("Icon", root.transform);
            var iconImage = iconGO.AddComponent<Image>();
            iconImage.color = Color.white;
            var iconLE = iconGO.AddComponent<LayoutElement>();
            iconLE.preferredWidth = 40;
            iconLE.preferredHeight = 40;
            iconLE.flexibleWidth = 0;
            iconLE.flexibleHeight = 0;

            // Info container
            var infoGO = CreateUIObject("Info", root.transform);
            var infoVL = infoGO.AddComponent<VerticalLayoutGroup>();
            infoVL.spacing = 2;
            infoVL.childForceExpandWidth = true;
            infoVL.childForceExpandHeight = false;
            infoVL.childControlWidth = true;
            infoVL.childControlHeight = true;
            var infoLE = infoGO.AddComponent<LayoutElement>();
            infoLE.flexibleWidth = 1;

            // Name
            var nameGO = CreateUIObject("Name", infoGO.transform);
            var nameText = nameGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Ability Name";
            nameText.fontSize = 14;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            nameText.enableWordWrapping = false;
            nameText.overflowMode = TextOverflowModes.Ellipsis;

            // Description
            var descGO = CreateUIObject("Description", infoGO.transform);
            var descText = descGO.AddComponent<TextMeshProUGUI>();
            descText.text = "Ability description";
            descText.fontSize = 11;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            descText.enableWordWrapping = true;

            // Add script and wire references
            var abilityUI = root.AddComponent<ui.selection.AbilityItemUI>();
            var so = new SerializedObject(abilityUI);
            so.FindProperty("_iconImage").objectReferenceValue = iconImage;
            so.FindProperty("_nameText").objectReferenceValue = nameText;
            so.FindProperty("_descriptionText").objectReferenceValue = descText;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Save prefab
            string path = $"{PrefabDir}/AbilityItem.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log($"Created: {path}");
            return prefab;
        }

        // ───────────────────────── EffectItem ─────────────────────────
        private static GameObject CreateEffectItemPrefab()
        {
            var root = new GameObject("EffectItem");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(350, 60);

            var layout = root.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 10;
            layout.padding = new RectOffset(5, 5, 5, 5);
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var le = root.AddComponent<LayoutElement>();
            le.preferredHeight = 60;

            // Icon
            var iconGO = CreateUIObject("Icon", root.transform);
            var iconImage = iconGO.AddComponent<Image>();
            iconImage.color = Color.white;
            var iconLE = iconGO.AddComponent<LayoutElement>();
            iconLE.preferredWidth = 40;
            iconLE.preferredHeight = 40;
            iconLE.flexibleWidth = 0;
            iconLE.flexibleHeight = 0;

            // Info container
            var infoGO = CreateUIObject("Info", root.transform);
            var infoVL = infoGO.AddComponent<VerticalLayoutGroup>();
            infoVL.spacing = 2;
            infoVL.childForceExpandWidth = true;
            infoVL.childForceExpandHeight = false;
            infoVL.childControlWidth = true;
            infoVL.childControlHeight = true;
            var infoLE = infoGO.AddComponent<LayoutElement>();
            infoLE.flexibleWidth = 1;

            // Header row (Name + Duration)
            var headerGO = CreateUIObject("Header", infoGO.transform);
            var headerHL = headerGO.AddComponent<HorizontalLayoutGroup>();
            headerHL.spacing = 8;
            headerHL.childControlWidth = true;
            headerHL.childControlHeight = true;
            headerHL.childForceExpandWidth = false;
            headerHL.childForceExpandHeight = false;

            // Name
            var nameGO = CreateUIObject("Name", headerGO.transform);
            var nameText = nameGO.AddComponent<TextMeshProUGUI>();
            nameText.text = "Effect Name";
            nameText.fontSize = 14;
            nameText.fontStyle = FontStyles.Bold;
            nameText.color = Color.white;
            var nameLE = nameGO.AddComponent<LayoutElement>();
            nameLE.flexibleWidth = 1;

            // Duration
            var durGO = CreateUIObject("Duration", headerGO.transform);
            var durText = durGO.AddComponent<TextMeshProUGUI>();
            durText.text = "5.0s";
            durText.fontSize = 12;
            durText.fontStyle = FontStyles.Italic;
            durText.color = new Color(1f, 1f, 0.5f, 1f);
            durText.horizontalAlignment = HorizontalAlignmentOptions.Right;
            var durLE = durGO.AddComponent<LayoutElement>();
            durLE.preferredWidth = 50;

            // Description
            var descGO = CreateUIObject("Description", infoGO.transform);
            var descText = descGO.AddComponent<TextMeshProUGUI>();
            descText.text = "Effect description";
            descText.fontSize = 11;
            descText.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            descText.enableWordWrapping = true;

            // Add script and wire references
            var effectUI = root.AddComponent<ui.selection.EffectItemUI>();
            var so = new SerializedObject(effectUI);
            so.FindProperty("_iconImage").objectReferenceValue = iconImage;
            so.FindProperty("_nameText").objectReferenceValue = nameText;
            so.FindProperty("_durationText").objectReferenceValue = durText;
            so.FindProperty("_descriptionText").objectReferenceValue = descText;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Save prefab
            string path = $"{PrefabDir}/EffectItem.prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log($"Created: {path}");
            return prefab;
        }

        // ───────────────────── SelectionInfoPanel ─────────────────────
        private static void CreateSelectionInfoPanelPrefab(GameObject abilityPrefab, GameObject effectPrefab)
        {
            // Root panel
            var root = new GameObject("SelectionInfoPanel");
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.sizeDelta = new Vector2(380, 600);
            rootRect.anchorMin = new Vector2(1, 0.5f);
            rootRect.anchorMax = new Vector2(1, 0.5f);
            rootRect.pivot = new Vector2(1, 0.5f);
            rootRect.anchoredPosition = new Vector2(-10, 0);

            root.AddComponent<CanvasRenderer>();
            var bgImage = root.AddComponent<Image>();
            bgImage.color = new Color(0.05f, 0.05f, 0.1f, 0.9f);

            var rootVL = root.AddComponent<VerticalLayoutGroup>();
            rootVL.padding = new RectOffset(15, 15, 15, 15);
            rootVL.spacing = 0;
            rootVL.childControlWidth = true;
            rootVL.childControlHeight = true;
            rootVL.childForceExpandWidth = true;
            rootVL.childForceExpandHeight = true;

            // ==================== TOWER PANEL ====================
            var towerPanel = CreateUIObject("TowerPanel", root.transform);
            var towerVL = towerPanel.AddComponent<VerticalLayoutGroup>();
            towerVL.padding = new RectOffset(0, 0, 0, 0);
            towerVL.spacing = 8;
            towerVL.childControlWidth = true;
            towerVL.childControlHeight = true;
            towerVL.childForceExpandWidth = true;
            towerVL.childForceExpandHeight = false;

            // --- Tower Header (Icon + Title) ---
            var towerHeader = CreateUIObject("TowerHeader", towerPanel.transform);
            var towerHeaderHL = towerHeader.AddComponent<HorizontalLayoutGroup>();
            towerHeaderHL.spacing = 12;
            towerHeaderHL.padding = new RectOffset(0, 0, 0, 5);
            towerHeaderHL.childControlWidth = true;
            towerHeaderHL.childControlHeight = true;
            towerHeaderHL.childForceExpandWidth = false;
            towerHeaderHL.childForceExpandHeight = false;
            var towerHeaderLE = towerHeader.AddComponent<LayoutElement>();
            towerHeaderLE.preferredHeight = 65;

            // Tower Icon
            var towerIconGO = CreateUIObject("TowerIcon", towerHeader.transform);
            var towerIconImage = towerIconGO.AddComponent<Image>();
            towerIconImage.color = Color.white;
            var towerIconLE = towerIconGO.AddComponent<LayoutElement>();
            towerIconLE.preferredWidth = 60;
            towerIconLE.preferredHeight = 60;
            towerIconLE.flexibleWidth = 0;

            // Tower Title group
            var towerTitleGrp = CreateUIObject("TowerTitleGroup", towerHeader.transform);
            var towerTitleVL = towerTitleGrp.AddComponent<VerticalLayoutGroup>();
            towerTitleVL.spacing = 2;
            towerTitleVL.childControlWidth = true;
            towerTitleVL.childControlHeight = true;
            towerTitleVL.childForceExpandWidth = true;
            towerTitleVL.childForceExpandHeight = false;
            var towerTitleLE = towerTitleGrp.AddComponent<LayoutElement>();
            towerTitleLE.flexibleWidth = 1;

            // Tower Name
            var towerNameGO = CreateUIObject("TowerName", towerTitleGrp.transform);
            var towerNameText = towerNameGO.AddComponent<TextMeshProUGUI>();
            towerNameText.text = "Tower Name";
            towerNameText.fontSize = 18;
            towerNameText.fontStyle = FontStyles.Bold;
            towerNameText.color = Color.white;

            // Tower Level
            var towerLevelGO = CreateUIObject("TowerLevel", towerTitleGrp.transform);
            var towerLevelText = towerLevelGO.AddComponent<TextMeshProUGUI>();
            towerLevelText.text = "Level 1";
            towerLevelText.fontSize = 14;
            towerLevelText.color = new Color(0.7f, 0.85f, 1f, 1f);

            // --- Separator ---
            CreateSeparator(towerPanel.transform);

            // --- Tower Stats ---
            var towerStats = CreateUIObject("TowerStats", towerPanel.transform);
            var towerStatsVL = towerStats.AddComponent<VerticalLayoutGroup>();
            towerStatsVL.spacing = 4;
            towerStatsVL.childControlWidth = true;
            towerStatsVL.childControlHeight = true;
            towerStatsVL.childForceExpandWidth = true;
            towerStatsVL.childForceExpandHeight = false;

            var towerDamageText = CreateStatText("TowerDamage", "Damage: 0", towerStats.transform);
            var towerAtkSpdText = CreateStatText("TowerAttackSpeed", "Attack Speed: 0", towerStats.transform);
            var towerRangeText = CreateStatText("TowerAttackRange", "Range: 0.0", towerStats.transform);

            // --- Abilities Header ---
            var abilitiesHdr = CreateUIObject("AbilitiesHeader", towerPanel.transform);
            var abilitiesHdrText = abilitiesHdr.AddComponent<TextMeshProUGUI>();
            abilitiesHdrText.text = "Abilities";
            abilitiesHdrText.fontSize = 14;
            abilitiesHdrText.fontStyle = FontStyles.Bold;
            abilitiesHdrText.color = new Color(1f, 0.85f, 0.4f, 1f);
            var abilitiesHdrLE = abilitiesHdr.AddComponent<LayoutElement>();
            abilitiesHdrLE.preferredHeight = 22;

            // --- Abilities Container ---
            var abilitiesContainer = CreateUIObject("AbilitiesContainer", towerPanel.transform);
            var abilitiesVL = abilitiesContainer.AddComponent<VerticalLayoutGroup>();
            abilitiesVL.spacing = 4;
            abilitiesVL.childControlWidth = true;
            abilitiesVL.childControlHeight = true;
            abilitiesVL.childForceExpandWidth = true;
            abilitiesVL.childForceExpandHeight = false;
            var abilitiesCSF = abilitiesContainer.AddComponent<ContentSizeFitter>();
            abilitiesCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var abilitiesLE = abilitiesContainer.AddComponent<LayoutElement>();
            abilitiesLE.flexibleHeight = 1;

            // --- Select Tower Button ---
            var selectBtnGO = CreateUIObject("SelectTowerButton", towerPanel.transform);
            var selectBtnImage = selectBtnGO.AddComponent<Image>();
            selectBtnImage.color = new Color(0.15f, 0.6f, 0.15f, 1f);
            var selectBtn = selectBtnGO.AddComponent<Button>();
            selectBtn.targetGraphic = selectBtnImage;
            var selectBtnLE = selectBtnGO.AddComponent<LayoutElement>();
            selectBtnLE.preferredHeight = 45;
            selectBtnLE.minHeight = 45;

            var selectBtnTextGO = CreateUIObject("Text", selectBtnGO.transform);
            var selectBtnTextRect = selectBtnTextGO.GetComponent<RectTransform>();
            selectBtnTextRect.anchorMin = Vector2.zero;
            selectBtnTextRect.anchorMax = Vector2.one;
            selectBtnTextRect.offsetMin = Vector2.zero;
            selectBtnTextRect.offsetMax = Vector2.zero;
            var selectBtnText = selectBtnTextGO.AddComponent<TextMeshProUGUI>();
            selectBtnText.text = "SELECT";
            selectBtnText.fontSize = 20;
            selectBtnText.fontStyle = FontStyles.Bold;
            selectBtnText.color = Color.white;
            selectBtnText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            selectBtnText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // ==================== ENEMY PANEL ====================
            var enemyPanel = CreateUIObject("EnemyPanel", root.transform);
            var enemyVL = enemyPanel.AddComponent<VerticalLayoutGroup>();
            enemyVL.padding = new RectOffset(0, 0, 0, 0);
            enemyVL.spacing = 8;
            enemyVL.childControlWidth = true;
            enemyVL.childControlHeight = true;
            enemyVL.childForceExpandWidth = true;
            enemyVL.childForceExpandHeight = false;

            // --- Enemy Header ---
            var enemyHeader = CreateUIObject("EnemyHeader", enemyPanel.transform);
            var enemyHeaderHL = enemyHeader.AddComponent<HorizontalLayoutGroup>();
            enemyHeaderHL.spacing = 12;
            enemyHeaderHL.padding = new RectOffset(0, 0, 0, 5);
            enemyHeaderHL.childControlWidth = true;
            enemyHeaderHL.childControlHeight = true;
            enemyHeaderHL.childForceExpandWidth = false;
            enemyHeaderHL.childForceExpandHeight = false;
            var enemyHeaderLE = enemyHeader.AddComponent<LayoutElement>();
            enemyHeaderLE.preferredHeight = 65;

            // Enemy Icon
            var enemyIconGO = CreateUIObject("EnemyIcon", enemyHeader.transform);
            var enemyIconImage = enemyIconGO.AddComponent<Image>();
            enemyIconImage.color = Color.white;
            var enemyIconLE = enemyIconGO.AddComponent<LayoutElement>();
            enemyIconLE.preferredWidth = 60;
            enemyIconLE.preferredHeight = 60;
            enemyIconLE.flexibleWidth = 0;

            // Enemy Name
            var enemyNameGO = CreateUIObject("EnemyName", enemyHeader.transform);
            var enemyNameText = enemyNameGO.AddComponent<TextMeshProUGUI>();
            enemyNameText.text = "Enemy Name";
            enemyNameText.fontSize = 18;
            enemyNameText.fontStyle = FontStyles.Bold;
            enemyNameText.color = Color.white;
            enemyNameText.verticalAlignment = VerticalAlignmentOptions.Middle;
            var enemyNameLE = enemyNameGO.AddComponent<LayoutElement>();
            enemyNameLE.flexibleWidth = 1;

            // --- Separator ---
            CreateSeparator(enemyPanel.transform);

            // --- Health Bar ---
            var healthBarSection = CreateUIObject("HealthBarSection", enemyPanel.transform);
            var healthBarSectionLE = healthBarSection.AddComponent<LayoutElement>();
            healthBarSectionLE.preferredHeight = 28;

            var sliderGO = CreateUIObject("HealthBar", healthBarSection.transform);
            var sliderRect = sliderGO.GetComponent<RectTransform>();
            sliderRect.anchorMin = Vector2.zero;
            sliderRect.anchorMax = Vector2.one;
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            var slider = sliderGO.AddComponent<Slider>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            // Slider Background
            var sliderBg = CreateUIObject("Background", sliderGO.transform);
            var sliderBgRect = sliderBg.GetComponent<RectTransform>();
            sliderBgRect.anchorMin = Vector2.zero;
            sliderBgRect.anchorMax = Vector2.one;
            sliderBgRect.offsetMin = Vector2.zero;
            sliderBgRect.offsetMax = Vector2.zero;
            var sliderBgImage = sliderBg.AddComponent<Image>();
            sliderBgImage.color = new Color(0.2f, 0.05f, 0.05f, 1f);

            // Fill Area
            var fillArea = CreateUIObject("Fill Area", sliderGO.transform);
            var fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            // Fill
            var fill = CreateUIObject("Fill", fillArea.transform);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fill.AddComponent<Image>();
            fillImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;

            // Health Text overlay
            var healthTextGO = CreateUIObject("HealthText", healthBarSection.transform);
            var healthTextRect = healthTextGO.GetComponent<RectTransform>();
            healthTextRect.anchorMin = Vector2.zero;
            healthTextRect.anchorMax = Vector2.one;
            healthTextRect.offsetMin = Vector2.zero;
            healthTextRect.offsetMax = Vector2.zero;
            var healthText = healthTextGO.AddComponent<TextMeshProUGUI>();
            healthText.text = "100 / 100";
            healthText.fontSize = 13;
            healthText.fontStyle = FontStyles.Bold;
            healthText.color = Color.white;
            healthText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            healthText.verticalAlignment = VerticalAlignmentOptions.Middle;
            healthText.enableWordWrapping = false;

            // --- Enemy Stats ---
            var enemyStats = CreateUIObject("EnemyStats", enemyPanel.transform);
            var enemyStatsVL = enemyStats.AddComponent<VerticalLayoutGroup>();
            enemyStatsVL.spacing = 4;
            enemyStatsVL.childControlWidth = true;
            enemyStatsVL.childControlHeight = true;
            enemyStatsVL.childForceExpandWidth = true;
            enemyStatsVL.childForceExpandHeight = false;

            var enemyDamageText = CreateStatText("EnemyDamage", "Damage: 0", enemyStats.transform);
            var enemyMoveSpeedText = CreateStatText("EnemyMoveSpeed", "Speed: 0.0", enemyStats.transform);
            var enemyArmorText = CreateStatText("EnemyArmor", "Armor: 0", enemyStats.transform);
            var enemyMagicResistText = CreateStatText("EnemyMagicResist", "Magic Resist: 0", enemyStats.transform);
            var enemyEvasionText = CreateStatText("EnemyEvasion", "Evasion: 0%", enemyStats.transform);

            // --- Effects Header ---
            var effectsHdr = CreateUIObject("EffectsHeader", enemyPanel.transform);
            var effectsHdrText = effectsHdr.AddComponent<TextMeshProUGUI>();
            effectsHdrText.text = "Active Effects";
            effectsHdrText.fontSize = 14;
            effectsHdrText.fontStyle = FontStyles.Bold;
            effectsHdrText.color = new Color(1f, 0.5f, 0.5f, 1f);
            var effectsHdrLE = effectsHdr.AddComponent<LayoutElement>();
            effectsHdrLE.preferredHeight = 22;

            // --- Effects Container ---
            var effectsContainer = CreateUIObject("EffectsContainer", enemyPanel.transform);
            var effectsVL = effectsContainer.AddComponent<VerticalLayoutGroup>();
            effectsVL.spacing = 4;
            effectsVL.childControlWidth = true;
            effectsVL.childControlHeight = true;
            effectsVL.childForceExpandWidth = true;
            effectsVL.childForceExpandHeight = false;
            var effectsCSF = effectsContainer.AddComponent<ContentSizeFitter>();
            effectsCSF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var effectsLE = effectsContainer.AddComponent<LayoutElement>();
            effectsLE.flexibleHeight = 1;

            // ==================== EMPTY PANEL ====================
            var emptyPanel = CreateUIObject("EmptyPanel", root.transform);
            var emptyVL = emptyPanel.AddComponent<VerticalLayoutGroup>();
            emptyVL.childAlignment = TextAnchor.MiddleCenter;
            emptyVL.childControlWidth = true;
            emptyVL.childControlHeight = true;
            emptyVL.childForceExpandWidth = true;
            emptyVL.childForceExpandHeight = true;

            var emptyTextGO = CreateUIObject("EmptyText", emptyPanel.transform);
            var emptyText = emptyTextGO.AddComponent<TextMeshProUGUI>();
            emptyText.text = "Select a tower\nor enemy";
            emptyText.fontSize = 18;
            emptyText.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            emptyText.fontStyle = FontStyles.Italic;
            emptyText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            emptyText.verticalAlignment = VerticalAlignmentOptions.Middle;

            // ==================== Wire SelectionInfoPanel script ====================
            var panelScript = root.AddComponent<ui.selection.SelectionInfoPanel>();
            var so = new SerializedObject(panelScript);

            // Main panels
            so.FindProperty("_towerPanel").objectReferenceValue = towerPanel;
            so.FindProperty("_enemyPanel").objectReferenceValue = enemyPanel;
            so.FindProperty("_emptyPanel").objectReferenceValue = emptyPanel;

            // Tower info
            so.FindProperty("_towerIcon").objectReferenceValue = towerIconImage;
            so.FindProperty("_towerName").objectReferenceValue = towerNameText;
            so.FindProperty("_towerLevel").objectReferenceValue = towerLevelText;
            so.FindProperty("_towerDamage").objectReferenceValue = towerDamageText;
            so.FindProperty("_towerAttackSpeed").objectReferenceValue = towerAtkSpdText;
            so.FindProperty("_towerAttackRange").objectReferenceValue = towerRangeText;
            so.FindProperty("_abilitiesContainer").objectReferenceValue = abilitiesContainer.transform;
            so.FindProperty("_abilityItemPrefab").objectReferenceValue = abilityPrefab;
            so.FindProperty("_selectTowerButton").objectReferenceValue = selectBtn;

            // Enemy info
            so.FindProperty("_enemyIcon").objectReferenceValue = enemyIconImage;
            so.FindProperty("_enemyName").objectReferenceValue = enemyNameText;
            so.FindProperty("_enemyHealthBar").objectReferenceValue = slider;
            so.FindProperty("_enemyHealthText").objectReferenceValue = healthText;
            so.FindProperty("_enemyDamage").objectReferenceValue = enemyDamageText;
            so.FindProperty("_enemyMoveSpeed").objectReferenceValue = enemyMoveSpeedText;
            so.FindProperty("_enemyArmor").objectReferenceValue = enemyArmorText;
            so.FindProperty("_enemyMagicResist").objectReferenceValue = enemyMagicResistText;
            so.FindProperty("_enemyEvasion").objectReferenceValue = enemyEvasionText;
            so.FindProperty("_effectsContainer").objectReferenceValue = effectsContainer.transform;
            so.FindProperty("_effectItemPrefab").objectReferenceValue = effectPrefab;

            so.ApplyModifiedPropertiesWithoutUndo();

            // Deactivate panels that should start hidden
            enemyPanel.SetActive(false);
            towerPanel.SetActive(false);

            // Start with the whole panel hidden
            root.SetActive(false);

            // Save prefab
            string path = $"{PrefabDir}/SelectionInfoPanel.prefab";
            PrefabUtility.SaveAsPrefabAsset(root, path);
            Object.DestroyImmediate(root);
            Debug.Log($"Created: {path}");
        }

        // ───────────────────────── Helpers ─────────────────────────

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.AddComponent<RectTransform>();
            go.transform.SetParent(parent, false);
            return go;
        }

        private static TMP_Text CreateStatText(string name, string defaultText, Transform parent)
        {
            var go = CreateUIObject(name, parent);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = defaultText;
            text.fontSize = 13;
            text.color = new Color(0.9f, 0.9f, 0.9f, 1f);
            text.enableWordWrapping = false;
            var le = go.AddComponent<LayoutElement>();
            le.preferredHeight = 18;
            return text;
        }

        private static void CreateSeparator(Transform parent)
        {
            var sep = CreateUIObject("Separator", parent);
            var sepImage = sep.AddComponent<Image>();
            sepImage.color = new Color(0.4f, 0.4f, 0.5f, 0.6f);
            var sepLE = sep.AddComponent<LayoutElement>();
            sepLE.preferredHeight = 1;
            sepLE.flexibleWidth = 1;
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
        }
    }
}
