using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

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
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.Transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.Transform.position) < 0.3f)
        {
            target.TakeDamage(damage);

            EnemyMovement move = (target as MonoBehaviour)?.GetComponent<EnemyMovement>();
            move?.ApplySlow(slowMultiplier, slowDuration);

            Destroy(gameObject);
        }
    }
}
