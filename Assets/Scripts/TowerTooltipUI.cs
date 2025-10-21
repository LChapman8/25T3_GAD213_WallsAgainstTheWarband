using UnityEngine;
using TMPro;

public class TowerTooltipUI : MonoBehaviour
{
    public static TowerTooltipUI Instance;

    [Header("UI References")]
    public GameObject tooltipObject;
    public TMP_Text towerNameText;
    public TMP_Text towerStatsText;
    public TMP_Text towerBioText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        tooltipObject.SetActive(false);
    }

    public void ShowTooltip(string towerName, string stats, string bio)
    {
        tooltipObject.SetActive(true);
        towerNameText.text = towerName;
        towerStatsText.text = stats;
        towerBioText.text = bio;
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}
