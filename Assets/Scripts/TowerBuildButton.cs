using UnityEngine;
using UnityEngine.UI;

public class TowerBuildButton : MonoBehaviour
{
    [Header("Tower Setup")]
    public GameObject towerPrefab;           // The full tower prefab (used when actually building)
    public GameObject ghostTowerPrefab;      // The transparent preview version
    public int towerCost = 50;               // Gold cost for this tower

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (TowerPlacementManager.Instance == null)
        {
            Debug.LogError("No TowerPlacementManager found in the scene!");
            return;
        }

        TowerPlacementManager.Instance.StartPlacingTower(towerPrefab, ghostTowerPrefab, towerCost);
    }
}
