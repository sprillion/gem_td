using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    public static class SetupSelectionPanel
    {
        [MenuItem("Tools/Setup Selection Panel In Scene")]
        public static void Setup()
        {
            // Find all canvases in the scene
            var canvases = Object.FindObjectsOfType<Canvas>(true);
            Canvas targetCanvas = null;

            foreach (var c in canvases)
            {
                if (c.renderMode == RenderMode.ScreenSpaceOverlay || c.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    targetCanvas = c;
                    break;
                }
            }

            // If no canvas found, create one
            if (targetCanvas == null)
            {
                Debug.LogWarning("No suitable Canvas found. Creating a new one.");
                var canvasGO = new GameObject("UICanvas");
                targetCanvas = canvasGO.AddComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                targetCanvas.sortingOrder = 10;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            // Check if panel already exists
            var existing = targetCanvas.GetComponentInChildren<ui.selection.SelectionInfoPanel>(true);
            if (existing != null)
            {
                Debug.Log("SelectionInfoPanel already exists in scene under: " + existing.transform.parent.name);
                return;
            }

            // Load prefab
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Prefabs/UI/SelectionInfoPanel.prefab");
            if (prefab == null)
            {
                Debug.LogError("SelectionInfoPanel prefab not found!");
                return;
            }

            // Instantiate under canvas
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, targetCanvas.transform);
            var rect = instance.GetComponent<RectTransform>();

            // Position on the right side of the screen
            rect.anchorMin = new Vector2(1, 0.5f);
            rect.anchorMax = new Vector2(1, 0.5f);
            rect.pivot = new Vector2(1, 0.5f);
            rect.anchoredPosition = new Vector2(-10, 0);
            rect.sizeDelta = new Vector2(380, 600);

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("SelectionInfoPanel added to canvas: " + targetCanvas.name);
        }
    }
}
