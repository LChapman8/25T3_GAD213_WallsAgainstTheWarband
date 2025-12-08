using UnityEngine;

public class BaseTriggerDamage : MonoBehaviour
{
    public int damagePerMinion = 1;
    public AudioSource audioSource;
    public AudioClip hitSFX;

    public ScreenShake screenShake; // reference your shake script
    private BaseHealth baseHealth;

    private void Start()
    {
        baseHealth = GetComponent<BaseHealth>();
    }

    public void OnTriggerEnter(Collider other)
    {
        // only detect minions
        MinionStats minion = other.GetComponent<MinionStats>();
        if (minion == null) return;

        // deal damage
        baseHealth.TakeDamage(damagePerMinion);

        // play sound
        if (audioSource != null && hitSFX != null)
            audioSource.PlayOneShot(hitSFX);

        // shake screen
        screenShake?.Shake();

        // delete minion
        Destroy(other.gameObject);
    }
}
