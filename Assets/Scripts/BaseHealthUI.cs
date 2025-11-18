using TMPro;
using UnityEngine;

public class BaseHealthUI : MonoBehaviour
{
    public BaseHealth baseHealth;
    public TextMeshProUGUI text;

    private void OnEnable()
    {
        baseHealth.OnHealthChanged += UpdateText;
    }

    private void OnDisable()
    {
        baseHealth.OnHealthChanged -= UpdateText;
    }

    private void UpdateText(int current, int max)
    {
        text.text = $"Base Health Remaining: {current} / {max}";
    }
}
