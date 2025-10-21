using UnityEngine;
using TMPro;

public class GoldManager : MonoBehaviour
{
    public int startingGold = 100;
    public int currentGold { get; private set; }

    public TextMeshProUGUI goldText; 

    private void Start()
    {
        currentGold = startingGold;
        UpdateGoldUI();
    }

    public bool SpendGold(int amount)
    {
        if (amount > currentGold)
            return false;

        currentGold -= amount;
        UpdateGoldUI();
        return true;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {currentGold}";
    }
}
