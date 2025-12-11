using UnityEngine;
using TMPro;
using System;

public class GoldManager : MonoBehaviour
{
    [Header("Gold Settings")]
    public int startingGold = 100;
    public TextMeshProUGUI goldText;

    [Header("Gold UI")]
    public GameObject goldPopupPrefab; 
    public Sprite goldIcon;            

    public int CurrentGold { get; private set; }

    public static GoldManager Instance; // singleton reference

    // --- EVENTS ---
    public static event Action<int> OnGoldChanged;

    private void Awake()
    {
        Instance = this;
        CurrentGold = startingGold;
    }

    private void Start()
    {
        UpdateGoldUI();
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public bool SpendGold(int amount)
    {
        if (amount > CurrentGold)
            return false;

        CurrentGold -= amount;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }

    public void AddGold(int amount, Vector3? popupPosition = null)
    {
        CurrentGold += amount;
        UpdateGoldUI();
        OnGoldChanged?.Invoke(CurrentGold);

        // Spawn gold popup if position is provided
        if (popupPosition.HasValue)
            SpawnGoldPopup(popupPosition.Value, amount);
    }

    private void UpdateGoldUI()
    {
        if (goldText != null)
            goldText.text = $"Gold: {CurrentGold}";
    }

    public void SpawnGoldPopup(Vector3 position, int amount)
    {
        if (goldPopupPrefab != null)
        {
            GameObject popup = Instantiate(goldPopupPrefab, position, Quaternion.identity);
            GoldPopup popupScript = popup.GetComponent<GoldPopup>();
            if (popupScript != null)
                popupScript.Initialize(amount, goldIcon);
        }
    }
}
