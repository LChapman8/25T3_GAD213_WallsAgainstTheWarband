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
    public float damageIncreasePercent = 0.25f; // 25% increase

    [Header("Range Indicator")]
    public GameObject rangeIndicator; // Drag your range object here

    private bool isUpgraded = false;
    private GoldManager goldManager;

    private void Start()
    {
        goldManager = Object.FindAnyObjectByType<GoldManager>();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        TowerMenuUI.Instance.Show(this);

        // Turn on range indicator when this tower is selected
        if (rangeIndicator != null)
            rangeIndicator.SetActive(true);
    }

    // Call this to turn off range when deselected / menu closes
    public void HideRangeIndicator()
    {
        if (rangeIndicator != null)
            rangeIndicator.SetActive(false);
    }

    public void UpgradeTower()
    {
        if (isUpgraded)
        {
            Debug.Log($"{towerName} is already upgraded!");
            FloatingTextManager.Instance?.ShowText($"{towerName} is already upgraded!");
            return;
        }

        if (goldManager == null || !goldManager.SpendGold(upgradeCost))
        {
            Debug.Log("Not enough gold to upgrade!");
            FloatingTextManager.Instance?.ShowText($"Not enough gold to upgrade {towerName}");
            return;
        }

        // Apply visual scale upgrade
        transform.localScale *= 1.2f;

        // Apply damage upgrade for any tower type
        bool upgraded = false;

        TowerAttack arrow = GetComponent<TowerAttack>();
        if (arrow != null)
        {
            arrow.damage = Mathf.CeilToInt(arrow.damage * (1f + damageIncreasePercent));
            upgraded = true;
        }

        if (upgraded)
        {
            isUpgraded = true;
            Debug.Log($"{towerName} upgraded! Damage increased by {damageIncreasePercent * 100}%");
            FloatingTextManager.Instance?.ShowText($"{towerName} upgraded! Damage increased by {damageIncreasePercent * 100}%");
        }
        else
        {
            Debug.LogWarning($"{towerName} has no recognized attack script to upgrade!");
        }
    }

    public void SellTower()
    {
        if (goldManager != null)
            goldManager.AddGold(sellValue);

        Debug.Log($"{towerName} sold for {sellValue} gold!");
        FloatingTextManager.Instance?.ShowText($"{towerName} sold for {sellValue} gold!");
        Destroy(gameObject);
    }
}
