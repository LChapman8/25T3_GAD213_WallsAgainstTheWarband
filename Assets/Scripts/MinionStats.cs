using UnityEngine;

public class MinionStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHealth = 10f;

    [Header("Runtime Stats")]
    public float maxHealth;
    public float currentHealth;

    [Header("UI")]
    public GameObject healthBarPrefab;
    private MinionHealthBar healthBarUI;

    public System.Action OnDeath;

    public void Initialise(int roundNumber)
    {
        maxHealth = CalculateScaledHealth(roundNumber);
        currentHealth = maxHealth;

        // Spawn and attach health bar
        if (healthBarPrefab != null)
        {
            GameObject ui = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 1.5f,
                Quaternion.identity
            );

            healthBarUI = ui.GetComponent<MinionHealthBar>();
            healthBarUI.stats = this;

            ui.transform.SetParent(transform);
        }
    }

    float CalculateScaledHealth(int round)
    {
        float multiplier = 1f + ((round - 1) * 0.25f); // +25% per wave
        return baseHealth * multiplier;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth < 0)
            currentHealth = 0;

        // Update health bar immediately
        if (healthBarUI != null)
        {
            healthBarUI.ForceUpdateUI();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        OnDeath?.Invoke();

        // Destroy health bar separately if needed
        if (healthBarUI != null)
        {
            Destroy(healthBarUI.gameObject);
        }

        Destroy(gameObject);
    }
}
