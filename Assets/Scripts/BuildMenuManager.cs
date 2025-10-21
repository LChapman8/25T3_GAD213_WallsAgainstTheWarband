using UnityEngine;
using UnityEngine.UI;

public class BuildMenuManager : MonoBehaviour
{
    public Button arrowTowerButton;
    public Button cannonTowerButton;
    public Button iceTowerButton;

    private string selectedTower = null;

    void Start()
    {
        arrowTowerButton.onClick.AddListener(() => SelectTower("Arrow"));
        cannonTowerButton.onClick.AddListener(() => SelectTower("Cannon"));
        iceTowerButton.onClick.AddListener(() => SelectTower("Ice"));
    }

    void SelectTower(string towerName)
    {
        selectedTower = towerName;
        Debug.Log($"Selected Tower: {towerName}");

        
    }

    public string GetSelectedTower()
    {
        return selectedTower;
    }

    public void ClearSelection()
    {
        selectedTower = null;
    }
}
