using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Penting untuk deteksi hover
using UnityEngine.SceneManagement;

public class StartButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    public string sceneToLoad = "Cutscene Car"; // Nama scene tujuan
    public float scaleFactor = 1.1f;            // Ukuran saat hover
    public float transitionSpeed = 0.2f;        // Kecepatan animasi

    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        // Tambahkan listener klik secara otomatis
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(StartGame);
        }
    }

    void Update()
    {
        // Animasi halus ke arah target scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, transitionSpeed);
    }

    // Fungsi yang dipanggil saat klik
    public void StartGame()
    {
        Debug.Log("Memulai Game...");
        SceneManager.LoadScene(sceneToLoad);
    }

    // Efek saat kursor masuk (Hover)
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleFactor;
    }

    // Efek saat kursor keluar
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}