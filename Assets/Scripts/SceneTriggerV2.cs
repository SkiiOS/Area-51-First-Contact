using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTriggerV2 : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Scene Settings")]
    [Tooltip("Drag scene dari Project window ke sini")]
    public SceneAsset targetScene;
#endif

    [Tooltip("Nama scene otomatis terisi")]
    public string sceneName;

    [Header("Trigger Settings")]
    [Tooltip("Tag untuk player (default: Player)")]
    public string playerTag = "Player";

    [Tooltip("Delay sebelum pindah scene (detik)")]
    public float transitionDelay = 0f;

    [Tooltip("Tampilkan debug log")]
    public bool showDebug = true;

    void OnValidate()
    {
#if UNITY_EDITOR
        // Auto-update sceneName saat scene di-drag
        if (targetScene != null)
        {
            sceneName = targetScene.name;
        }
#endif
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (showDebug)
                Debug.Log($"Player masuk trigger! Pindah ke scene: {sceneName}");

            if (transitionDelay > 0)
                Invoke(nameof(LoadScene), transitionDelay);
            else
                LoadScene();
        }
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene belum di-assign! Drag scene dari Project ke slot 'Target Scene'.");
        }
    }

#if UNITY_EDITOR
    // Custom Inspector untuk dropdown scene
    [CustomEditor(typeof(SceneTriggerV2))]
    public class SceneTriggerV2Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            SceneTriggerV2 trigger = (SceneTriggerV2)target;

            EditorGUILayout.LabelField("Drag Scene dari Project", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Drag file .unity dari Project window ke slot di bawah", MessageType.Info);

            // Scene Asset field
            SerializedProperty sceneProp = serializedObject.FindProperty("targetScene");
            EditorGUILayout.PropertyField(sceneProp);

            serializedObject.ApplyModifiedProperties();

            // Tampilkan nama scene (read-only)
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Scene Name (Auto):", trigger.sceneName);

            // Draw remaining fields
            EditorGUILayout.Space();
            DrawPropertiesExcluding(serializedObject, "targetScene", "m_Script");
        }
    }
#endif

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
