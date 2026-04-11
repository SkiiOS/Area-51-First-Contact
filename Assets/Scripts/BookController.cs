using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BookController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    public AudioSource audioSource;
    public string initialSceneName = "Start scene";

    [Header("Buttons")]
    public GameObject nextButton;
    public GameObject backButton;

    [Header("Settings")]
    public int totalPages = 4; // Sesuaikan dengan jumlah slide kamu
    private int currentPage = 0;

    [Header("Sound Clips")]
    public AudioClip flipSound;

    void Start()
    {
        // 1. Matikan animasi agar tidak auto-play di awal
        if (animator != null)
        {
            animator.enabled = true; // Pastikan animator aktif
            // Jika ingin benar-benar diam, pastikan State pertama di Animator adalah "Idle" 
            // atau state yang tidak memiliki animasi bergerak.
        }

        // 2. Set halaman ke 0 dan update tombol
        currentPage = 0;
        UpdateButtons();
    }

    void Update()
    {
        // Tombol ESC untuk kembali
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(initialSceneName);
        }
    }

    public void NextSlide()
    {
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            animator.SetTrigger("Next");
            PlayFlipSound();
            UpdateButtons();
        }
    }

    public void PreviousSlide()
    {
        if (currentPage > 0)
        {
            currentPage--;
            animator.SetTrigger("Back");
            PlayFlipSound();
            UpdateButtons();
        }
    }

    void UpdateButtons()
    {
        // Tombol Back HANYA muncul jika bukan di halaman pertama (halaman 0)
        if (backButton != null)
            backButton.SetActive(currentPage > 0);

        // Tombol Next HANYA muncul jika belum mencapai halaman terakhir
        if (nextButton != null)
            nextButton.SetActive(currentPage < totalPages - 1);
    }

    public void PlayFlipSound()
    {
        if (audioSource != null && flipSound != null)
        {
            audioSource.PlayOneShot(flipSound);
        }
    }
}