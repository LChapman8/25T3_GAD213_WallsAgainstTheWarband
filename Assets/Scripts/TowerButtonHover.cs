using UnityEngine;
using UnityEngine.EventSystems;

public class TowerButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(1, 2)] public string towerName;
    [TextArea(2, 4)] public string towerStats;
    [TextArea(3, 6)] public string towerBio;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TowerTooltipUI.Instance.ShowTooltip(towerName, towerStats, towerBio);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TowerTooltipUI.Instance.HideTooltip();
    }
}
