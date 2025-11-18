using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    public Transform cam;
    public float intensity = 0.2f;
    public float duration = 0.15f;

    public void Shake()
    {
        StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = cam.localPosition;
        float time = 0;

        while (time < duration)
        {
            cam.localPosition = originalPos + (Vector3)Random.insideUnitCircle * intensity;
            time += Time.deltaTime;
            yield return null;
        }

        cam.localPosition = originalPos;
    }
}
