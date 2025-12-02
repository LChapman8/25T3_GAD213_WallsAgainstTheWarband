using UnityEngine;

public class MinionStats : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseHealth = 10f;
    public float progressDistance;

    [Header("Audio")]
    public AudioClip deathSound;
    public AudioSource audioSource;

    [Header("Gold Drop")]
    public int goldOnDeath = 10;

    [Header("Runtime Stats")]
    public float maxHealth = 10f;
    public float currentHealth = 10f;

    [Header("UI")]
    public GameObject healthBarPrefab;
    private MinionHealthBar healthBarUI;

    public System.Action OnDeath;

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
            healthBarUI.stats = this;
            ui.transform.SetParent(transform, worldPositionStays: true);
            healthBarUI.ForceUpdateUI();
        }
    }

    float CalculateScaledHealth(int round)
    {
        float multiplier = 1f + ((round - 1) * 0.25f);
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
        // Play death sound at the minion position
        if (deathSound != null)
        {
            PlaySoundAtPosition.PlayClip(deathSound, transform.position);
        }

        // Give player gold + popup
        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldOnDeath, transform.position + Vector3.up * 1.5f);

        OnDeath?.Invoke();

        // Destroy health bar
        if (healthBarUI != null)
            Destroy(healthBarUI.gameObject);

        // Minion dies instantly
        Destroy(gameObject);
    }


}
