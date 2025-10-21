using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(1, 2)] public string towerName;
    [TextArea(2, 4)] public string towerStats;
    [TextArea(3, 6)] public string towerBio;

    private bool isHovering = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovering) return; // prevent multiple triggers
        isHovering = true;

        TowerTooltipUI.Instance.ShowTooltip(towerName, towerStats, towerBio);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        TowerTooltipUI.Instance.HideTooltip();
    }
}
