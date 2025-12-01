using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GoldPopup : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI amountText;
    public Image goldIconImage;

    [Header("Popup Settings")]
    public float floatSpeed = 1.5f;
    public float lifetime = 1f;

    private float elapsed = 0f;

    /// <summary>
    /// Initialize the popup with amount and optional icon sprite.
    /// </summary>
    public void Initialize(int amount, Sprite iconSprite = null)
    {
        if (amountText != null)
            amountText.text = $"+{amount}";

        if (goldIconImage != null && iconSprite != null)
            goldIconImage.sprite = iconSprite;
    }

    void Update()
    {
        // Move upwards in world space
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        elapsed += Time.deltaTime;
        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}
