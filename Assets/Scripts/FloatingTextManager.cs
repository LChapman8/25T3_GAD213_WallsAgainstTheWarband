using TMPro;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    public FloatingTextUI textPrefab;
    public RectTransform spawnParent;

    void Awake()
    {
        Instance = this;
    }

    public void ShowText(string message)
    {
        if (textPrefab == null || spawnParent == null)
            return;

        FloatingTextUI text = Instantiate(textPrefab, spawnParent);
        text.transform.localPosition = Vector3.zero;
        text.SetText(message);
    }
}
