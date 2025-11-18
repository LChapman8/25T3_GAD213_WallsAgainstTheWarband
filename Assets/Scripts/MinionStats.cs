using UnityEngine;

public class MinionStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHealth = 10f;

    [Header("Runtime Stats")]
    public float maxHealth = 10f; //{ get; private set; }
    public float currentHealth = 10f; //{ get; private set; }

    [Header("UI")]
    public GameObject healthBarPrefab;
    private MinionHealthBar healthBarUI;

    public System.Action OnDeath;

    public void Initialise(int roundNumber)
    {
        // Set stats
        maxHealth = CalculateScaledHealth(roundNumber);
        currentHealth = maxHealth;

        // Spawn health bar
        if (healthBarPrefab != null)
        {
            GameObject ui = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 1.5f,
                Quaternion.identity
            );

            // Assign reference
            healthBarUI = ui.GetComponent<MinionHealthBar>();
            healthBarUI.stats = this;

            // Parent to minion correctly (no scaling issues)
            ui.transform.SetParent(transform, worldPositionStays: true);

            // Update UI instantly so no background shows
            healthBarUI.ForceUpdateUI();
        }
        else
        {
            Debug.LogWarning("Minion has no health bar prefab assigned.", this);
        }
    }

    float CalculateScaledHealth(int round)
    {
        float multiplier = 1f + ((round - 1) * 0.25f); // +25% HP each wave
        return baseHealth * multiplier;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        if (healthBarUI != null)
            healthBarUI.ForceUpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        OnDeath?.Invoke();

        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        Destroy(gameObject);
    }
}
