using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float speed = 8f;
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

    private IEnemy target;
    private int damage;
    private Vector3 destination;
    private bool detached;

    public void Initialize(IEnemy target, int damage)
    {
        this.target = target;
        this.damage = damage;
        destination = target.Transform.position;
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

        if (Vector3.Distance(transform.position, destination) < 0.25f)
        {
            if (!detached && target != null)
            {
                target.TakeDamage(damage);
                EnemyMovement move = (target as MonoBehaviour)?.GetComponent<EnemyMovement>();
                move?.ApplySlow(slowMultiplier, slowDuration);
            }

            Destroy(gameObject);
        }
    }

    void Move()
    {
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }
}
