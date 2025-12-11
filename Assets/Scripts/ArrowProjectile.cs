using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 12f;

    private IEnemy target;
    private int damage;
    private Vector3 destination;
    private bool detached;

    [Header("Orientation")]
    public float forwardOffset = 90f;

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
        RotateTowardsTarget();

        if (Vector3.Distance(transform.position, destination) < 0.25f)
        {
            if (!detached && target != null)
                target.TakeDamage(damage);

            Destroy(gameObject);
        }
    }

    void Move()
    {
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    void RotateTowardsTarget()
    {
        Vector3 dir = destination - transform.position;
        if (dir != Vector3.zero)
        {
            // Rotate to face the target
            Quaternion lookRot = Quaternion.LookRotation(dir);
            // Apply forward offset if model's tip isn't along Z+
            transform.rotation = lookRot * Quaternion.Euler(0f, forwardOffset, 0f);
        }
    }
}
