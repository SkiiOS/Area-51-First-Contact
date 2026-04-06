using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilemapSortingLayerSetup : MonoBehaviour
{
    [System.Serializable]
    public class TilemapLayerConfig
    {
        public string sortingLayerName = "Ground";
        public int orderInLayer = 0;
        public string description = "";
    }

    [Header("Sorting Layer Configurations")]
    public List<TilemapLayerConfig> layerConfigs = new List<TilemapLayerConfig>
    {
        new TilemapLayerConfig { sortingLayerName = "Background", orderInLayer = -10, description = "Far background" },
        new TilemapLayerConfig { sortingLayerName = "Ground", orderInLayer = 0, description = "Main ground tiles" },
        new TilemapLayerConfig { sortingLayerName = "Ground", orderInLayer = 1, description = "Ground decorations" },
        new TilemapLayerConfig { sortingLayerName = "Objects", orderInLayer = 0, description = "Player, enemies, items" },
        new TilemapLayerConfig { sortingLayerName = "Foreground", orderInLayer = 0, description = "Foreground elements" },
        new TilemapLayerConfig { sortingLayerName = "UI", orderInLayer = 0, description = "UI elements" }
    };

    [Header("Auto Setup Options")]
    [Tooltip("Automatically detect and setup all tilemaps in children")]
    public bool autoDetectChildren = true;

    [Tooltip("Apply setup on start (runtime) or use button (editor)")]
    public bool applyOnStart = false;

    [Tooltip("Set Sort Order mode for tilemaps")]
    public TilemapRenderer.SortOrder sortOrder = TilemapRenderer.SortOrder.BottomRight;

    void Start()
    {
        if (applyOnStart)
        {
            SetupAllTilemaps();
        }
    }

    [ContextMenu("Setup All Tilemaps")]
    public void SetupAllTilemaps()
    {
        TilemapRenderer[] renderers;

        if (autoDetectChildren)
        {
            renderers = GetComponentsInChildren<TilemapRenderer>(true);
        }
        else
        {
            renderers = FindObjectsByType<TilemapRenderer>(FindObjectsInactive.Include);
        }

        Debug.Log($"Found {renderers.Length} Tilemap Renderers");

        for (int i = 0; i < renderers.Length && i < layerConfigs.Count; i++)
        {
            ApplyConfig(renderers[i], layerConfigs[i]);
        }

        // Log warning if more tilemaps than configs
        if (renderers.Length > layerConfigs.Count)
        {
            Debug.LogWarning($"Found {renderers.Length} tilemaps but only {layerConfigs.Count} configs. Some tilemaps were not configured.");
        }
    }

    void ApplyConfig(TilemapRenderer renderer, TilemapLayerConfig config)
    {
        Undo.RecordObject(renderer, "Setup Sorting Layer");

        renderer.sortingLayerName = config.sortingLayerName;
        renderer.sortingOrder = config.orderInLayer;
        renderer.sortOrder = sortOrder;

        Debug.Log($"Setup {renderer.name}: Layer='{config.sortingLayerName}', Order={config.orderInLayer}");
    }

    [ContextMenu("Create Sorting Layers")]
    void CreateSortingLayersMenu()
    {
#if UNITY_EDITOR
        CreateSortingLayers();
#endif
    }

#if UNITY_EDITOR
    [ContextMenu("Print Current Sorting Layers")]
    void PrintCurrentSortingLayers()
    {
        Debug.Log("Current Sorting Layers:");
        for (int i = 0; i < SortingLayer.layers.Length; i++)
        {
            var layer = SortingLayer.layers[i];
            Debug.Log($"  [{i}] {layer.name} (ID: {layer.id})");
        }
    }

    [ContextMenu("Auto Assign Config by Name")]
    void AutoAssignByName()
    {
        TilemapRenderer[] renderers = GetComponentsInChildren<TilemapRenderer>(true);

        foreach (var renderer in renderers)
        {
            string name = renderer.name.ToLower();
            TilemapLayerConfig config = null;

            // Auto-detect based on name
            if (name.Contains("background") || name.Contains("back"))
                config = layerConfigs.Find(c => c.sortingLayerName == "Background");
            else if (name.Contains("ground") || name.Contains("floor"))
                config = layerConfigs.Find(c => c.sortingLayerName == "Ground" && c.orderInLayer == 0);
            else if (name.Contains("decor") || name.Contains("detail"))
                config = layerConfigs.Find(c => c.sortingLayerName == "Ground" && c.orderInLayer == 1);
            else if (name.Contains("object") || name.Contains("entity"))
                config = layerConfigs.Find(c => c.sortingLayerName == "Objects");
            else if (name.Contains("foreground") || name.Contains("front"))
                config = layerConfigs.Find(c => c.sortingLayerName == "Foreground");

            if (config != null)
            {
                ApplyConfig(renderer, config);
            }
        }
    }

    public void CreateSortingLayers()
    {
        // Get existing layers
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
        );

        SerializedProperty sortingLayers = tagManager.FindProperty("m_SortingLayers");

        // Add default layers if not exist
        string[] defaultLayers = { "Background", "Ground", "Objects", "Foreground", "UI" };

        foreach (string layerName in defaultLayers)
        {
            if (!LayerExists(sortingLayers, layerName))
            {
                AddLayer(sortingLayers, layerName);
                Debug.Log($"Created Sorting Layer: {layerName}");
            }
        }

        tagManager.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
    }

    bool LayerExists(SerializedProperty layers, string name)
    {
        for (int i = 0; i < layers.arraySize; i++)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(i);
            if (layer.FindPropertyRelative("name").stringValue == name)
                return true;
        }
        return false;
    }

    void AddLayer(SerializedProperty layers, string name)
    {
        layers.InsertArrayElementAtIndex(layers.arraySize);
        SerializedProperty newLayer = layers.GetArrayElementAtIndex(layers.arraySize - 1);
        newLayer.FindPropertyRelative("name").stringValue = name;
        newLayer.FindPropertyRelative("uniqueID").intValue = GetRandomLayerID();
    }

    int GetRandomLayerID()
    {
        return Random.Range(10000000, 99999999);
    }

    [CustomEditor(typeof(TilemapSortingLayerSetup))]
    public class TilemapSortingLayerSetupEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("1. Create Sorting Layers", GUILayout.Height(30)))
            {
                var setup = (TilemapSortingLayerSetup)target;
                setup.CreateSortingLayers();
                EditorUtility.DisplayDialog("Success", "Sorting layers created! Check Project Settings > Tags and Layers", "OK");
            }

            if (GUILayout.Button("2. Setup All Tilemaps", GUILayout.Height(30)))
            {
                var setup = (TilemapSortingLayerSetup)target;
                setup.SetupAllTilemaps();
            }

            if (GUILayout.Button("3. Auto Assign by Name", GUILayout.Height(30)))
            {
                var setup = (TilemapSortingLayerSetup)target;
                setup.AutoAssignByName();
            }

            if (GUILayout.Button("Print Current Layers", GUILayout.Height(25)))
            {
                var setup = (TilemapSortingLayerSetup)target;
                setup.PrintCurrentSortingLayers();
            }
        }
    }
#endif
}
