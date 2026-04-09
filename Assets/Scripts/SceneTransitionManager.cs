using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    // Static variable untuk menyimpan posisi spawn di scene berikutnya
    public static Vector3 spawnPosition = Vector3.zero;
    public static string previousScene = "";

    // Sistem baru: cari spawn point berdasarkan nama di scene tujuan
    public static string targetSpawnPointName = "SpawnPoint";
    public static Vector3 targetSpawnOffset = Vector3.zero;
    public static bool useNamedSpawnPoint = false;

    [Header("Spawn Points (Optional)")]
    [Tooltip("Nama scene -> posisi spawn")]
    public SpawnPointData[] spawnPoints;

    [Header("Debug")]
    public bool showDebug = true;

    void Awake()
    {
        // Jangan pindahkan player di Awake, tunggu semua objek siap
        // Pindahkan ke Start() dengan delay
    }

    void Start()
    {
        // Tunggu 1 frame untuk memastikan semua objek sudah siap
        StartCoroutine(SpawnPlayerDelayed());
    }

    System.Collections.IEnumerator SpawnPlayerDelayed()
    {
        // Tunggu 1 frame
        yield return null;

        // Cari player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) yield break;

        // CEK: Apakah ini scene pertama (tidak ada previous scene)?
        if (string.IsNullOrEmpty(previousScene))
        {
            // Scene pertama, biarkan player di posisi awal (tidak pindah)
            if (showDebug)
                Debug.Log($"[SPAWN] First scene - player stays at initial position: {player.transform.position}");
            yield break;
        }

        // SISTEM BARU: Cari spawn point berdasarkan nama
        if (useNamedSpawnPoint)
        {
            GameObject spawnObj = GameObject.Find(targetSpawnPointName);
            if (spawnObj != null)
            {
                Vector3 finalPos = spawnObj.transform.position + targetSpawnOffset;
                player.transform.position = finalPos;
                if (showDebug)
                    Debug.Log($"[SPAWN] Player moved to '{targetSpawnPointName}': {finalPos} (offset: {targetSpawnOffset})");
            }
            else
            {
                Debug.LogWarning($"[SPAWN] Spawn point '{targetSpawnPointName}' not found! Player stays at initial position.");
            }

            useNamedSpawnPoint = false; // Reset
        }
        // SISTEM LAMA: Spawn position langsung
        else if (spawnPosition != Vector3.zero)
        {
            player.transform.position = spawnPosition;
            if (showDebug)
                Debug.Log($"[SPAWN] Player moved to: {spawnPosition} (from scene: {previousScene})");
        }
        else
        {
            // Cari default spawn point di scene ini
            Transform defaultSpawn = FindSpawnPoint(SceneManager.GetActiveScene().name);
            if (defaultSpawn != null)
            {
                player.transform.position = defaultSpawn.position;
                if (showDebug)
                    Debug.Log($"[SPAWN] Player spawned at default: {defaultSpawn.position}");
            }
        }

        // Reset setelah digunakan
        spawnPosition = Vector3.zero;
        previousScene = ""; // Reset biar scene selanjutnya tahu ini scene pertama
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
        useNamedSpawnPoint = false;

        Debug.Log($"Spawn position set to: {position} for scene transition from {fromScene}");
    }

    // Panggil ini untuk cari spawn point berdasarkan nama di scene tujuan
    public static void SetTargetSpawnPointName(string spawnName, Vector3 offset)
    {
        targetSpawnPointName = spawnName;
        targetSpawnOffset = offset;
        previousScene = "transition"; // Marker bahwa ini pindah scene
        useNamedSpawnPoint = true;

        Debug.Log($"[SPAWN] Will look for spawn point named '{spawnName}' with offset {offset}");
    }
}

[System.Serializable]
public class SpawnPointData
{
    public string sceneName;
    public Transform spawnPoint;
}
