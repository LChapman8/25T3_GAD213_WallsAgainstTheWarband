using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float explosionRadius = 2.5f;
    public GameObject explosionVFX;
    public AudioClip explosionSound;

    private IEnemy target;
    private int damage;

    public void Initialize(IEnemy target, int damage)
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

        Vector3 dir = (target.Transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.Transform.position) < 0.4f)
            Explode();
    }

    void Explode()
    {
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }

        if (explosionSound != null)
            PlaySoundAtPosition.PlayClip(explosionSound, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            IEnemy enemy = hit.GetComponent<MonoBehaviour>() as IEnemy;
            enemy?.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
