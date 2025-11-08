using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerMenuUI : MonoBehaviour
{
    public static TowerMenuUI Instance;

    [Header("UI Elements")]
    public GameObject panel;
    public TMP_Text towerNameText;
    public Button upgradeButton;
    public Button sellButton;

    private TowerClickable currentTower;

    void Awake()
    {
        Instance = this;
        if (panel != null)
            panel.SetActive(false);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(OnUpgradePressed);

        if (sellButton != null)
            sellButton.onClick.AddListener(OnSellPressed);
    }

    public void Show(TowerClickable tower)
    {
        currentTower = tower;
        if (panel != null)
            panel.SetActive(true);

        if (towerNameText != null)
            towerNameText.text = tower.towerName; 
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        currentTower = null;
    }

    private void OnUpgradePressed()
    {
        if (currentTower != null)
            currentTower.UpgradeTower();
    }

    private void OnSellPressed()
    {
        if (currentTower != null)
            currentTower.SellTower();
        Hide();
    }
}
