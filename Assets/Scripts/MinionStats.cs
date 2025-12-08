using UnityEngine;

public class MinionStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHealth = 50f;
    public float progressDistance; // needed for tower targeting

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioSource audioSource;

    [Header("Gold Drop")]
    public int goldOnDeath = 10;

    [Header("Runtime Stats")]
    public float maxHealth;
    public float currentHealth;

    [Header("UI")]
    public GameObject healthBarPrefab;
    private MinionHealthBar healthBarUI;

    public System.Action OnDeath;

    // Initialise for the given wave
    public void Initialise(int roundNumber)
    {
        maxHealth = CalculateScaledHealth(roundNumber);
        currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject ui = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 1.5f,
                Quaternion.identity
            );

            healthBarUI = ui.GetComponent<MinionHealthBar>();
            if (healthBarUI != null)
            {
                healthBarUI.stats = this;
                ui.transform.SetParent(transform, worldPositionStays: true);
                healthBarUI.ForceUpdateUI();
            }
            else
            {
                Debug.LogWarning("Health bar prefab missing MinionHealthBar component!");
            }
        }
    }

    float CalculateScaledHealth(int round)
    {
        float multiplier = 1f + ((round - 1) * 0.70f); // +25% per round
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
        if (deathSound != null)
            PlaySoundAtPosition.PlayClip(deathSound, transform.position);

        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldOnDeath, transform.position + Vector3.up * 1.5f);

        OnDeath?.Invoke();

        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        Destroy(gameObject);
    }
}
