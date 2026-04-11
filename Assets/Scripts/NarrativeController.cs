using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; // Tambahkan ini untuk pindah scene

public class NarrativeController : MonoBehaviour
{
    [Header("Daftar Urutan Narasi")]
    [TextArea(3, 5)]
    public List<string> urutanNarasi = new List<string>();

    [Header("Pengaturan Animasi")]
    public float kecepatanFade = 1.0f;
    public float jedaAntarTeks = 2.0f;

    [Header("Pengaturan Scene")]
    public string namaSceneTujuan; // Ketik nama scene tujuan di Inspector
    public float jedaSebelumPindah = 1.0f;

    [Header("Komponen")]
    public TextMeshProUGUI tmpTeks;

    private void Start()
    {
        if (tmpTeks == null) tmpTeks = GetComponent<TextMeshProUGUI>();

        if (urutanNarasi.Count > 0)
        {
            StartCoroutine(PutarSemuaNarasi());
        }
    }

    private IEnumerator PutarSemuaNarasi()
    {
        foreach (string narasi in urutanNarasi)
        {
            // 1. Fade In
            yield return StartCoroutine(FadeTeks(narasi, 0, 1));

            // 2. Jeda baca
            yield return new WaitForSeconds(jedaAntarTeks);

            // 3. Fade Out
            yield return StartCoroutine(FadeTeks(narasi, 1, 0));
        }

        // --- SEMUA TEKS SELESAI ---
        yield return new WaitForSeconds(jedaSebelumPindah);

        if (!string.IsNullOrEmpty(namaSceneTujuan))
        {
            SceneManager.LoadScene(namaSceneTujuan);
        }
        else
        {
            Debug.LogWarning("Nama scene tujuan belum diisi di Inspector!");
        }
    }

    private IEnumerator FadeTeks(string teks, float startAlpha, float endAlpha)
    {
        tmpTeks.text = teks;
        Color warnaAsli = tmpTeks.color;
        float durasi = 0;

        while (durasi < 1.0f)
        {
            durasi += Time.deltaTime * kecepatanFade;
            float alphaSaatIni = Mathf.Lerp(startAlpha, endAlpha, durasi);
            tmpTeks.color = new Color(warnaAsli.r, warnaAsli.g, warnaAsli.b, alphaSaatIni);
            yield return null;
        }

        tmpTeks.color = new Color(warnaAsli.r, warnaAsli.g, warnaAsli.b, endAlpha);
    }
}