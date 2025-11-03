using UnityEngine;

public class TowerUpgradeUI : MonoBehaviour
{
    public GameObject upgradePanel;

    private static TowerUpgradeUI instance;

    void Awake()
    {
        instance = this;
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    public static void ShowUI()
    {
        if (instance != null && instance.upgradePanel != null)
            instance.upgradePanel.SetActive(true);
    }

    public static void HideUI()
    {
        if (instance != null && instance.upgradePanel != null)
            instance.upgradePanel.SetActive(false);
    }
}
