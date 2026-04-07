using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerDropdown : MonoBehaviour
{
    [Header("Scene Selection")]
    [Tooltip("Pilih scene dari dropdown")]
    public int sceneIndex = 0;  // Index di Build Settings

    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public float transitionDelay = 0f;
    public bool showDebug = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            if (showDebug)
                Debug.Log($"Player masuk trigger! Pindah ke scene index: {sceneIndex}");

            if (transitionDelay > 0)
                Invoke(nameof(LoadScene), transitionDelay);
            else
                LoadScene();
        }
    }

    void LoadScene()
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Scene index {sceneIndex} tidak valid! Cek Build Settings (File > Build Settings).");
        }
    }

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
