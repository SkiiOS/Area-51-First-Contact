using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public Image fadeImage;
    public float duration = 1f;

    public IEnumerator FadeIn()
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, 1 - t / duration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut()
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, t / duration);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }
}