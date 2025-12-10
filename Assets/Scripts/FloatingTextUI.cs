using System.Collections;
using TMPro;
using UnityEngine;

public class FloatingTextUI : MonoBehaviour
{
    [Header("Animation")]
    public float floatDistance = 40f;
    public float duration = 1.2f;

    private TMP_Text text;
    private CanvasGroup canvasGroup;
    private Vector3 startPos;
    private Vector3 endPos;

    void Awake()
    {
        text = GetComponent<TMP_Text>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        startPos = transform.localPosition;
        endPos = startPos + Vector3.up * floatDistance;

        StartCoroutine(Animate());
    }

    public void SetText(string message)
    {
        if (text != null)
            text.text = message;
    }

    private IEnumerator Animate()
    {
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;

            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            canvasGroup.alpha = 1f - t;

            time += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
