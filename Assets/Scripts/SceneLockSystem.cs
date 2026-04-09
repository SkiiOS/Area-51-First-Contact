using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLockSystem : MonoBehaviour
{
    [Header("Scene Lock Settings")]
    [Tooltip("Nama scene yang akan di-lock (tidak bisa keluar)")]
    public string lockedSceneName;

    [Tooltip("Player tidak bisa keluar sampai syarat terpenuhi")]
    public bool canExit = false;

    [Tooltip("Pesan debug saat mencoba keluar tapi terkunci")]
    public string lockedMessage = "Kamu perlu berinteraksi dulu sebelum keluar!";

    [Header("Exit Trigger")]
    [Tooltip("Collider untuk keluar (akan di-disable jika locked)")]
    public Collider2D exitTrigger;

    [Header("Visual Feedback")]
    [Tooltip("Tampilkan UI atau efek saat terkunci")]
    public bool showFeedback = true;

    // Static - persists across scenes
    public static SceneLockSystem Instance;
    public static bool IsLocked = false;
    public static string CurrentLockedScene = "";

    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Cek apakah scene ini locked
        if (SceneManager.GetActiveScene().name == CurrentLockedScene && IsLocked)
        {
            LockExit();
        }
    }

    void Start()
    {
        // Auto-find exit trigger kalau belum di-assign
        if (exitTrigger == null)
        {
            // Cari object dengan nama "ExitTrigger" atau "DoorExit"
            GameObject exit = GameObject.Find("ExitTrigger");
            if (exit != null)
            {
                exitTrigger = exit.GetComponent<Collider2D>();
            }
        }

        // Lock scene saat masuk
        if (!string.IsNullOrEmpty(lockedSceneName))
        {
            LockScene(lockedSceneName);
        }
    }

    // Lock scene supaya tidak bisa keluar
    public void LockScene(string sceneName)
    {
        IsLocked = true;
        CurrentLockedScene = sceneName;
        lockedSceneName = sceneName;

        LockExit();

        Debug.Log($"[SCENE LOCK] {sceneName} terkunci! Player tidak bisa keluar.");
    }

    // Unlock scene supaya bisa keluar
    public void UnlockScene()
    {
        IsLocked = false;
        CurrentLockedScene = "";
        canExit = true;

        UnlockExit();

        Debug.Log("[SCENE LOCK] Scene di-unlock! Player sekarang bisa keluar.");
    }

    void LockExit()
    {
        if (exitTrigger != null)
        {
            exitTrigger.enabled = false;
            Debug.Log($"[SCENE LOCK] Exit trigger '{exitTrigger.name}' di-disable");
        }
        else
        {
            Debug.LogWarning("[SCENE LOCK] Exit trigger tidak ditemukan!");
        }

        if (showFeedback)
        {
            Debug.Log($"[SCENE LOCK] {lockedMessage}");
        }
    }

    void UnlockExit()
    {
        if (exitTrigger != null)
        {
            exitTrigger.enabled = true;
            Debug.Log($"[SCENE LOCK] Exit trigger '{exitTrigger.name}' di-enable");
        }
    }

    // Panggil ini dari dialog atau interaction
    public void AllowExit()
    {
        UnlockScene();
    }

    // Panggil ini dari button atau event
    public void OnUnlockButtonPressed()
    {
        AllowExit();
    }

    // Cek status
    public bool IsExitLocked()
    {
        return IsLocked && SceneManager.GetActiveScene().name == CurrentLockedScene;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
