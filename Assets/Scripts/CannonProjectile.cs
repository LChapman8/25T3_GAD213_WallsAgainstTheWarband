using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    public float speed = 6f;
    public float explosionRadius = 2.5f;
    public GameObject explosionVFX;
    public AudioClip explosionSound;

    private IEnemy target;
    private int damage;
    private Vector3 destination;
    private bool detached;

    private Vector3 spinAxis;
    private float spinSpeed;

    public void Initialize(IEnemy target, int damage)
    {
        this.target = target;
        this.damage = damage;
        destination = target.Transform.position;

        // Random spin setup
        spinAxis = Random.onUnitSphere;   // random direction
        spinSpeed = Random.Range(180f, 540f); // degrees per second
    }

    void Update()
    {
        if (!detached && target != null && target.CurrentHealth > 0)
        {
            destination = target.Transform.position;
        }
        else
        {
            detached = true;
        }

        Move();

        if (Vector3.Distance(transform.position, destination) < 0.4f)
            Explode();
    }

    void Move()
    {
        Vector3 dir = (destination - transform.position).normalized;

        // Move forward
        transform.position += dir * speed * Time.deltaTime;

        // Spin randomly
        transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
    }

    void Explode()
    {
        if (explosionVFX != null)
            Destroy(Instantiate(explosionVFX, transform.position, Quaternion.identity), 2f);

        if (explosionSound != null)
            PlaySoundAtPosition.PlayClip(explosionSound, transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            MonoBehaviour[] comps = hit.GetComponents<MonoBehaviour>();
            foreach (var c in comps)
            {
                if (c is IEnemy enemy)
                {
                    enemy.TakeDamage(damage);
                    break;
                }
            }
        }

        Destroy(gameObject);
    }
}
