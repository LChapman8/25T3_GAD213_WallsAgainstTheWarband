using UnityEngine;

public class BossStats : MonoBehaviour, IEnemy
{
    public float maxHealth = 5000f;
    public float currentHealth;

    [Header("Base Damage")]
    public int damageToBase = 50;

    [Header("Audio")]
    public AudioClip deathSound;

    [HideInInspector] public float progressDistance = 0f;

    private EnemyMovement movement;

    void Awake()
    {
        currentHealth = maxHealth;
        movement = GetComponent<EnemyMovement>();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (deathSound != null)
            PlaySoundAtPosition.PlayClip(deathSound, transform.position);

        Destroy(gameObject);
        Debug.Log("BOSS DEFEATED – YOU WIN");
    }

    public void ReachBase()
    {
        BaseHealth baseHealth = FindObjectOfType<BaseHealth>();
        ScreenShake shake = FindObjectOfType<ScreenShake>();

        if (baseHealth != null)
            baseHealth.TakeDamage(damageToBase);

        shake?.Shake();

        AudioSource audioSource = FindObjectOfType<AudioSource>();
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        Destroy(gameObject);
        Debug.Log("BOSS REACHED BASE – GAME OVER");
    }

    // ---- IEnemy Implementation ----
    public float CurrentHealth => currentHealth;
    public float Progress => progressDistance;
    public Transform Transform => transform;
}
