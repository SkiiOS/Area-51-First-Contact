using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class InstantSceneLoader : MonoBehaviour
{
    public string gameplaySceneName;
    private AsyncOperation asyncLoad;

    void Start()
    {
        // Langsung muat scene gameplay di background saat menu muncul
        StartCoroutine(PreloadScene());
    }

    IEnumerator PreloadScene()
    {
        asyncLoad = SceneManager.LoadSceneAsync(gameplaySceneName);

        // JANGAN aktifkan scene dulu, biarkan dia standby di 90%
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // Jika sudah 90%, berarti sudah siap "meledak" (instan)
            if (asyncLoad.progress >= 0.9f)
            {
                Debug.Log("Scene Gameplay Siap!");
                yield break;
            }
            yield return null;
        }
    }

    // Panggil fungsi ini di Button Start (OnClick)
    public void StartGame()
    {
        if (asyncLoad != null)
        {
            // Langsung pindah tanpa loading lagi
            asyncLoad.allowSceneActivation = true;
        }
    }
}