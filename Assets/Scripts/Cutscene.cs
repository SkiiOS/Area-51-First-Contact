using System.Collections;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Playables;

public class AdvancedCutscene : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject car;

    public UIFade fade;
    public CameraShake shake;
    public SlowMotion slowmo;

    private bool played = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !played)
        {
            played = true;
            StartCoroutine(PlayCutscene());
        }
    }

    IEnumerator PlayCutscene()
    {
        // Fade dari hitam ke scene
        yield return fade.FadeIn();

        car.SetActive(true);
        director.Play();

        // Slow motion awal
        StartCoroutine(slowmo.DoSlowMotion(0.5f, 1f));

        // Delay lalu shake
        yield return new WaitForSeconds(1f);
        StartCoroutine(shake.Shake());

        // Tunggu cutscene
        yield return new WaitForSeconds(3f);

        // Fade ke hitam
        yield return fade.FadeOut();
    }
}