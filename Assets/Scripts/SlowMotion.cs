using UnityEngine;
using System.Collections;

public class SlowMotion : MonoBehaviour
{
    public IEnumerator DoSlowMotion(float scale, float duration)
    {
        Time.timeScale = scale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}