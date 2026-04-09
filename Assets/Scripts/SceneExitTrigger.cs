using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitTrigger : MonoBehaviour
{
    [Header("Exit Settings")]
    public string targetSceneName;
    public string playerTag = "Player";

    [Header("Lock Check")]
    [Tooltip("Cek apakah scene terkunci sebelum keluar")]
    public bool checkLock = true;

    [Tooltip("Pesan saat scene terkunci")]
    public string lockedMessage = "Pintu terkunci! Kamu perlu berinteraksi dulu.";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Cek apakah scene terkunci
        if (checkLock && SceneLockSystem.IsLocked)
        {
            // Scene terkunci, tidak bisa keluar
            Debug.Log($"[EXIT] {lockedMessage}");

            // Trigger event untuk dialog/UI (opsional)
            OnLockedExitAttempt();

            return;
        }

        // Scene tidak terkunci, bisa keluar
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Debug.Log($"[EXIT] Pindah ke scene: {targetSceneName}");
            SceneManager.LoadScene(targetSceneName);
        }
    }

    void OnLockedExitAttempt()
    {
        // Di sini bisa trigger dialog atau UI
        // Contoh: DialogManager.Instance.ShowDialog("Pintu terkunci!");

        Debug.Log("[EXIT] Trigger locked exit event - tampilkan dialog di sini!");
    }
}
