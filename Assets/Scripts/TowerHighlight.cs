using UnityEngine;

public class TowerHighlight : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Renderer[] renderersToHighlight;
    public Color highlightColor = Color.yellow;
    private Color[] originalColors;
    private bool isHighlighted = false;

    void Start()
    {
        if (renderersToHighlight == null || renderersToHighlight.Length == 0)
        {
            Renderer r = GetComponentInChildren<Renderer>();
            if (r != null)
                renderersToHighlight = new Renderer[] { r };
        }

        originalColors = new Color[renderersToHighlight.Length];
        for (int i = 0; i < renderersToHighlight.Length; i++)
        {
            if (renderersToHighlight[i].material.HasProperty("_Color"))
                originalColors[i] = renderersToHighlight[i].material.color;
        }
    }

    public void SetHighlighted(bool value)
    {
        if (isHighlighted == value) return;
        isHighlighted = value;

        for (int i = 0; i < renderersToHighlight.Length; i++)
        {
            if (renderersToHighlight[i].material.HasProperty("_Color"))
            {
                renderersToHighlight[i].material.color = value
                    ? highlightColor
                    : originalColors[i];
            }
        }
    }
}
