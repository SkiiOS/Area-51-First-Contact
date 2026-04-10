using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public CharacterStats playerStats; // Tarik file PlayerStats ke sini
    public Image barFillImage;         // Tarik objek BarFill ke sini

    void Update()
    {
        if (playerStats != null && barFillImage != null)
        {
            // Mengatur panjang bar sesuai persentase darah
            barFillImage.fillAmount = playerStats.GetHealthPercentage();
        }
    }
}