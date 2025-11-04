using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickable : MonoBehaviour
{
    [Header("Tower Info")]
    public string towerName = "Basic Tower";
    public int sellValue = 50;
    public int upgradeCost = 100;

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
    }

    public void UpgradeTower()
    {
        if (isUpgraded)
        {
            Debug.Log($"{towerName} is already upgraded!");
            return;
        }

        if (goldManager == null || !goldManager.SpendGold(upgradeCost))
        {
            Debug.Log("Not enough gold to upgrade!");
            return;
        }

        // Example upgrade behavior:
        transform.localScale *= 1.2f;
        isUpgraded = true;
        Debug.Log($"{towerName} upgraded! Cost: {upgradeCost}");
    }

    public void SellTower()
    {
        if (goldManager != null)
            goldManager.AddGold(sellValue);

        Debug.Log($"{towerName} sold for {sellValue} gold!");
        Destroy(gameObject);
    }
}
