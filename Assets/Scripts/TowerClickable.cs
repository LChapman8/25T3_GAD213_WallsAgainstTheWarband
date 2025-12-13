using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickable : MonoBehaviour
{
    [Header("Tower Info")]
    public string towerName = "Tower";
    public int sellValue = 50;
    public int upgradeCost = 100;

    [Header("Upgrade Settings")]
    [Range(0f, 1f)]
    public float damageIncreasePercent = 0.25f;

    [Header("Range Indicator")]
    public GameObject rangeIndicator;

    private bool isUpgraded = false;
    private GoldManager goldManager;

    
    private static TowerClickable currentlySelectedTower;

    private void Start()
    {
        goldManager = Object.FindAnyObjectByType<GoldManager>();

        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        
        if (currentlySelectedTower != null && currentlySelectedTower != this)
        {
            currentlySelectedTower.HideRangeIndicator();
        }

        
        currentlySelectedTower = this;

        TowerMenuUI.Instance.Show(this);

        if (rangeIndicator != null)
            rangeIndicator.SetActive(true);
    }

    
    public void HideRangeIndicator()
    {
        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);

        if (currentlySelectedTower == this)
            currentlySelectedTower = null;
    }

  
    public static void HideAllRanges()
    {
        if (currentlySelectedTower != null)
        {
            currentlySelectedTower.HideRangeIndicator();
            currentlySelectedTower = null;
        }
    }

    public void UpgradeTower()
    {
        if (isUpgraded)
        {
            FloatingTextManager.Instance?.ShowText($"{towerName} is already upgraded!");
            return;
        }

        if (goldManager == null || !goldManager.SpendGold(upgradeCost))
        {
            FloatingTextManager.Instance?.ShowText($"Not enough gold to upgrade {towerName}");
            return;
        }

        transform.localScale *= 1.2f;

        TowerAttack attack = GetComponent<TowerAttack>();
        if (attack != null)
        {
            attack.damage = Mathf.CeilToInt(attack.damage * (1f + damageIncreasePercent));
            isUpgraded = true;
            FloatingTextManager.Instance?.ShowText($"{towerName} upgraded!");
        }
    }

    public void SellTower()
    {
        if (goldManager != null)
            goldManager.AddGold(sellValue);

        FloatingTextManager.Instance?.ShowText($"{towerName} sold for {sellValue} gold!");
        Destroy(gameObject);
    }
}
