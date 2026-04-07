using UnityEngine;
using UnityEngine.SceneManagement;

public class MinecraftCredits : MonoBehaviour
{
    [Header("Settings")]
    public RectTransform textTransform; // Tarik objek CreditText ke sini
    public float scrollSpeed = 50f;      // Kecepatan jalan teks
    public string menuSceneName = "StartScene"; // Nama scene untuk kembali ke menu

    [Header("Input")]
    public bool canSkip = true;

    private bool isFinished = false;

    void Update()
    {
        // Gerakkan teks ke atas setiap frame
        if (!isFinished)
        {
            textTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        }

        // Cek jika teks sudah melewati batas atas layar (misal tinggi teks 5000)
        // Sesuaikan angka 2000 dengan titik di mana teks kamu benar-benar habis
        if (textTransform.anchoredPosition.y > 2500f)
        {
            FinishCredits();
        }

        // Fitur Skip ala Minecraft (Tekan sembarang tombol/Esc)
        if (canSkip && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space)))
        {
            FinishCredits();
        }
    }

    void FinishCredits()
    {
        isFinished = true;
        // Kembali ke Start Menu
        SceneManager.LoadScene(menuSceneName);
    }
}