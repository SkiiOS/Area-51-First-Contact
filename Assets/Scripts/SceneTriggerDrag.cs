using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SceneTriggerDrag : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Drag Scene dari Folder Project")]
    [Tooltip("Drag file .unity dari Project window ke sini")]
    public SceneAsset targetScene;
#endif

    [Header("Scene Name (Auto)")]
    [Tooltip("Nama scene otomatis terisi saat drag")]
    public string sceneName;

    [Header("Settings")]
    public string playerTag = "Player";
    public float transitionDelay = 0f;

    [Header("Fade Transition")]
    [Tooltip("Aktifkan fade in/out")]
    public bool useFade = true;
    [Tooltip("Durasi fade (detik)")]
    public float fadeDuration = 1f;
    [Tooltip("Warna fade (biasanya hitam)")]
    public Color fadeColor = Color.black;

    private Canvas fadeCanvas;
    private Image fadeImage;
    private static bool isFading = false;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (targetScene != null)
        {
            sceneName = targetScene.name;
        }
    }
#endif

    void Start()
    {
        // Setup fade canvas kalau belum ada
        if (useFade && fadeCanvas == null)
        {
            SetupFadeCanvas();
        }

        // Fade in saat scene dimulai
        if (useFade && !isFading)
        {
            StartCoroutine(FadeIn());
        }
    }

    void SetupFadeCanvas()
    {
        // Buat canvas untuk fade
        GameObject canvasGO = new GameObject("FadeCanvas");
        fadeCanvas = canvasGO.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 999; // Paling depan

        // Buat image hitam
        GameObject imageGO = new GameObject("FadeImage");
        imageGO.transform.SetParent(canvasGO.transform, false);
        fadeImage = imageGO.AddComponent<Image>();
        fadeImage.color = fadeColor;
        fadeImage.raycastTarget = false;

        // Full screen
        RectTransform rect = fadeImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;

        // Awalnya transparan
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isFading)
        {
            if (useFade)
            {
                StartCoroutine(FadeOutAndLoad());
            }
            else if (transitionDelay > 0)
            {
                Invoke(nameof(LoadScene), transitionDelay);
            }
            else
            {
                LoadScene();
            }
        }
    }

    IEnumerator FadeOutAndLoad()
    {
        isFading = true;

        // Setup canvas kalau belum ada
        if (fadeCanvas == null)
            SetupFadeCanvas();

        // Fade out (hitam)
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = elapsed / fadeDuration;
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1);

        // Delay kalau ada
        if (transitionDelay > 0)
            yield return new WaitForSeconds(transitionDelay);

        // Load scene
        LoadScene();
    }

    IEnumerator FadeIn()
    {
        isFading = true;

        // Setup canvas kalau belum ada
        if (fadeCanvas == null)
            SetupFadeCanvas();

        // Awalnya hitam
        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1);

        // Fade in (transparan)
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1 - (elapsed / fadeDuration);
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0);
        isFading = false;
    }

    void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene belum di-assign! Drag scene dari Project window ke slot 'Target Scene'.");
            isFading = false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
