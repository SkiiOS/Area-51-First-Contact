using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // Static variable untuk menyimpan posisi spawn di scene berikutnya
    public static Vector3 spawnPosition = Vector3.zero;
    public static string previousScene = "";

    [Header("Spawn Points (Optional)")]
    [Tooltip("Nama scene -> posisi spawn")]
    public SpawnPointData[] spawnPoints;

    [Header("Debug")]
    public bool showDebug = true;

    void Awake()
    {
        // Cari player dan pindahkan ke spawn position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            if (spawnPosition != Vector3.zero)
            {
                player.transform.position = spawnPosition;
                if (showDebug)
                    Debug.Log($"Player spawned at: {spawnPosition} (from previous scene: {previousScene})");
            }
            else
            {
                // Cari spawn point di scene ini
                Transform spawnPoint = FindSpawnPoint(SceneManager.GetActiveScene().name);
                if (spawnPoint != null)
                {
                    player.transform.position = spawnPoint.position;
                    if (showDebug)
                        Debug.Log($"Player spawned at default spawn point: {spawnPoint.position}");
                }
            }
        }

        // Reset spawn position setelah digunakan
        spawnPosition = Vector3.zero;
    }

    Transform FindSpawnPoint(string sceneName)
    {
        // Cari dari array spawn points
        foreach (var data in spawnPoints)
        {
            if (data.sceneName == sceneName)
                return data.spawnPoint;
        }

        // Cari object dengan nama "SpawnPoint" di scene
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
            return spawn.transform;

        // Cari object dengan tag "SpawnPoint"
        spawn = GameObject.FindWithTag("SpawnPoint");
        if (spawn != null)
            return spawn.transform;

        return null;
    }

    // Panggil ini sebelum pindah scene untuk tentukan posisi spawn
    public static void SetSpawnPosition(Vector3 position, string fromScene)
    {
        spawnPosition = position;
        previousScene = fromScene;

        Debug.Log($"Spawn position set to: {position} for scene transition from {fromScene}");
    }
}

[System.Serializable]
public class SpawnPointData
{
    public string sceneName;
    public Transform spawnPoint;
}
