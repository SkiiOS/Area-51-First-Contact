using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    public Animator faderAnimator;
    private string sceneToLoad;

    public void FadeToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        // Memicu animasi dari bening ke hitam
        faderAnimator.SetTrigger("StartFade");
    }

    // FUNGSI UTAMA: Ini yang dipanggil oleh Animation Event
    public void OnFadeComplete()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}