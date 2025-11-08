using UnityEngine;

public class TowerHighlight : MonoBehaviour
{
    /// <summary>
    /// This script should be fully functional but cannot work out for the life of me why it isnt working in engine, problem for project 3 in my polish stage.
    /// </summary>
    public Renderer[] renderersToHighlight;
    public Material outlineMaterial;

    private Material[][] originalMaterials;
    private bool isHighlighted;

    void Start()
    {
        if (renderersToHighlight == null || renderersToHighlight.Length == 0)
        {
            Renderer r = GetComponentInChildren<Renderer>();
            if (r != null)
                renderersToHighlight = new Renderer[] { r };
        }

        originalMaterials = new Material[renderersToHighlight.Length][];
        for (int i = 0; i < renderersToHighlight.Length; i++)
            originalMaterials[i] = renderersToHighlight[i].materials;
    }

    public void SetHighlighted(bool value)
    {
        if (isHighlighted == value) return;
        isHighlighted = value;

        for (int i = 0; i < renderersToHighlight.Length; i++)
        {
            if (value)
            {
                // Add outline material
                var newMats = new Material[originalMaterials[i].Length + 1];
                for (int j = 0; j < originalMaterials[i].Length; j++)
                    newMats[j] = originalMaterials[i][j];
                newMats[newMats.Length - 1] = outlineMaterial;
                renderersToHighlight[i].materials = newMats;
            }
            else
            {
                // Revert
                renderersToHighlight[i].materials = originalMaterials[i];
            }
        }
    }
}
