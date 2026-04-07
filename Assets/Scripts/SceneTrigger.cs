using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Nama scene yang akan dimuat (harus ada di Build Settings)")]
    public string sceneName;

    [Tooltip("Tag untuk player (default: Player)")]
    public string playerTag = "Player";

    [Header("Optional")]
    [Tooltip("Delay sebelum pindah scene (detik)")]
    public float transitionDelay = 0f;

    [Tooltip("Tampilkan debug log")]
    public bool showDebug = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang nyentuh adalah player
        if (other.CompareTag(playerTag))
        {
            if (showDebug)
                Debug.Log($"Player masuk trigger! Pindah ke scene: {sceneName}");

            // Pindah scene
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
            Debug.LogError("Scene name belum diisi! Masukkan nama scene di Inspector.");
        }
    }

    // Visualisasi trigger di editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider2D>().bounds.size);
    }
}
