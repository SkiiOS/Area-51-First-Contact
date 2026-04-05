using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFade : MonoBehaviour
{
    public Image panel;
    public float duration = 1f;

    void Awake()
    {
        if (panel == null)
            panel = GetComponent<Image>();
    }

    public IEnumerator FadeIn() // hitam ? transparan
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = 1 - (t / duration);
            panel.color = new Color(0, 0, 0, a);
            yield return null;
        }
        panel.color = new Color(0, 0, 0, 0);
    }

    public IEnumerator FadeOut() // transparan ? hitam
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = t / duration;
            panel.color = new Color(0, 0, 0, a);
            yield return null;
        }
        panel.color = new Color(0, 0, 0, 1);
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }
}