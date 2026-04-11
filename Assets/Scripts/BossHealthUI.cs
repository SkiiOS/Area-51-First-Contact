using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Image fillImage; // Tarik gambar 'Fill' ke sini
    public GameObject healthBarPanel; // Tarik panel 'BossHealthBar' ke sini

    // Fungsi untuk mengupdate bar
    public void UpdateBossHealth(float current, float max)
    {
        fillImage.fillAmount = current / max;
    }

    // Fungsi untuk menyembunyikan bar jika bos belum muncul atau sudah mati
    public void SetActive(bool state)
    {
        healthBarPanel.SetActive(state);
    }
}