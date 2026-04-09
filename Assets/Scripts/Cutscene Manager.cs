using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Settings")]
    public Animator animator;
    public string animationName; // Nama state animasi di Animator
    public string nextSceneName; // Nama scene tujuan

    void Start()
    {
        // Memulai proses cutscene saat scene dimulai
        StartCoroutine(PlayCutsceneAndChangeScene());
    }

    IEnumerator PlayCutsceneAndChangeScene()
    {
        // 1. Pastikan animator tidak null
        if (animator == null) animator = GetComponent<Animator>();

        // 2. Mainkan animasi
        animator.Play(animationName);

        // 3. Tunggu sebentar agar animator melakukan transisi ke state baru
        yield return new WaitForEndOfFrame();

        // 4. Ambil durasi animasi yang sedang berjalan
        float duration = animator.GetCurrentAnimatorStateInfo(0).length;

        // 5. Tunggu selama durasi animasi tersebut
        yield return new WaitForSeconds(duration);

        // 6. Pindah Scene
        SceneManager.LoadScene(nextSceneName);
    }
}