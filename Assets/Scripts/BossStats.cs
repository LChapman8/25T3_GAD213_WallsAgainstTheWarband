using UnityEngine;
using System;

public class BossStats : MonoBehaviour, IEnemy, IHealth
{
    [Header("Health")]
    public float maxHealth = 5000f;
    public float currentHealth;
    public GameObject healthBarPrefab;
    private WorldSpaceHealthBar healthBarUI;

    [Header("Base Damage")]
    public int damageToBase = 50;

    [Header("Audio")]
    public AudioClip deathSound;

    private EnemyMovement movement;

    public static event Action OnBossDefeated;

    void Awake()
    {
        currentHealth = maxHealth;
        movement = GetComponent<EnemyMovement>();
    }

    void Start()
    {
        if (healthBarPrefab != null)
        {
            GameObject ui = Instantiate(
                healthBarPrefab,
                transform.position + Vector3.up * 3f,
                Quaternion.identity
            );

            healthBarUI = ui.GetComponent<WorldSpaceHealthBar>();
            if (healthBarUI != null)
            {
                healthBarUI.healthSource = this;
                ui.transform.SetParent(transform, worldPositionStays: true);
                healthBarUI.ForceUpdateUI();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        healthBarUI?.ForceUpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (deathSound != null)
            PlaySoundAtPosition.PlayClip(deathSound, transform.position);

        // Only trigger normal victory if endless mode is NOT active
        EndlessModeManager endless = UnityEngine.Object.FindFirstObjectByType<EndlessModeManager>();
        if (endless == null || !endless.endlessActive)
            OnBossDefeated?.Invoke();

        Destroy(gameObject);
        Debug.Log("BOSS DEFEATED");
    }

    public void ReachBase()
    {
        BaseHealth baseHealth = UnityEngine.Object.FindFirstObjectByType<BaseHealth>();
        ScreenShake shake = UnityEngine.Object.FindFirstObjectByType<ScreenShake>();

        if (baseHealth != null)
            baseHealth.TakeDamage(damageToBase);

        shake?.Shake();

        Destroy(gameObject);
        Debug.Log("BOSS REACHED BASE – GAME OVER");
    }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float Progress => movement != null ? movement.Progress : float.MinValue;
    public Transform Transform => transform;
}
