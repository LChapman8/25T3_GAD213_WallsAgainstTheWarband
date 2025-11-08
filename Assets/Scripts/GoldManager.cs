using UnityEngine;
using TMPro;
using System; // for Action events

public class GoldManager : MonoBehaviour
{
    [Header("Gold Settings")]
    public int startingGold = 100;
    public TextMeshProUGUI goldText;

    public int CurrentGold { get; private set; }

    // --- EVENTS ---
    public static event Action<int> OnGoldChanged; 

    private void Awake()
    {
        CurrentGold = startingGold;
    }

    private void Start()
    {
        UpdateGoldUI();
        OnGoldChanged?.Invoke(CurrentGold); // let UI know the initial amount
    }

    public bool SpendGold(int amount)
    {
        if (amount > CurrentGold)
            return false;

        CurrentGold -= amount;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(CurrentGold); // notify listeners (buttons, UI, etc.)
        return true;
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(CurrentGold);
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {CurrentGold}";
    }
}
