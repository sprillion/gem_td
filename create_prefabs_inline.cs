#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

// Execute this in Unity Editor
EditorApplication.delayCall += () =>
{
    Debug.Log("Creating AbilityItem prefab...");

    // Create AbilityItem
    var abilityItem = new GameObject("AbilityItem");
    var rect = abilityItem.AddComponent<RectTransform>();
    rect.sizeDelta = new Vector2(350, 60);

    var layout = abilityItem.AddComponent<HorizontalLayoutGroup>();
    layout.spacing = 10;
    layout.childControlWidth = true;
    layout.childControlHeight = true;
    layout.padding = new RectOffset(5, 5, 5, 5);

    var layoutElem = abilityItem.AddComponent<LayoutElement>();
    layoutElem.preferredHeight = 60;

    // Icon
    var icon = new GameObject("Icon");
    icon.transform.SetParent(abilityItem.transform);
    var iconRect = icon.AddComponent<RectTransform>();
    iconRect.sizeDelta = new Vector2(40, 40);
    icon.AddComponent<Image>().color = Color.white;
    var iconLayout = icon.AddComponent<LayoutElement>();
    iconLayout.preferredWidth = 40;
    iconLayout.preferredHeight = 40;

    // Info
    var info = new GameObject("Info");
    info.transform.SetParent(abilityItem.transform);
    info.AddComponent<RectTransform>();
    var infoLayout = info.AddComponent<VerticalLayoutGroup>();
    infoLayout.spacing = 2;
    infoLayout.childForceExpandWidth = true;

    // Name
    var name = new GameObject("Name");
    name.transform.SetParent(info.transform);
    name.AddComponent<RectTransform>();
    var nameText = name.AddComponent<TextMeshProUGUI>();
    nameText.text = "Ability Name";
    nameText.fontSize = 14;
    nameText.fontStyle = FontStyles.Bold;

    // Description
    var desc = new GameObject("Description");
    desc.transform.SetParent(info.transform);
    desc.AddComponent<RectTransform>();
    var descText = desc.AddComponent<TextMeshProUGUI>();
    descText.text = "Description";
    descText.fontSize = 11;
    descText.color = new Color(0.8f, 0.8f, 0.8f);

    // Add component
    abilityItem.AddComponent<ui.selection.AbilityItemUI>();

    // Save prefab
    string dir = "Assets/Resources/Prefabs/UI";
    if (!System.IO.Directory.Exists(dir))
        System.IO.Directory.CreateDirectory(dir);

    string path = dir + "/AbilityItem.prefab";
    PrefabUtility.SaveAsPrefabAsset(abilityItem, path);
    Object.DestroyImmediate(abilityItem);

    Debug.Log("AbilityItem prefab created at: " + path);
    AssetDatabase.Refresh();
};
#endif
