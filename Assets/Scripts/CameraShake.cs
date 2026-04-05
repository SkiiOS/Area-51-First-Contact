using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public CinemachineCamera cam;
    public float duration = 0.5f;
    public float intensity = 2f;

    public IEnumerator Shake()
    {
        var noise = cam.GetComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise != null)
        {
            noise.AmplitudeGain = intensity;

            yield return new WaitForSeconds(duration);

            noise.AmplitudeGain = 0f;
        }
    }
}