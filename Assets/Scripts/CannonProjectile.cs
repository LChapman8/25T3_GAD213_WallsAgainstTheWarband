using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 6f;           // slower than arrow
    public float explosionRadius = 2.5f;

    [Header("VFX")]
    public GameObject explosionVFX;

    [Header("Audio")]
    public AudioClip explosionSound;

    private MinionStats target;
    private int damage;

    public void Initialize(MinionStats target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target == null)
        {
            Explode();
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.4f)
            Explode();
    }

    void Explode()
    {
        // Explosion VFX
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        // Explosion sound
        if (explosionSound != null)
            PlaySoundAtPosition.PlayClip(explosionSound, transform.position);

        // AOE damage
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            MinionStats minion = hit.GetComponent<MinionStats>();
            if (minion != null && minion.currentHealth > 0)
            {
                minion.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
