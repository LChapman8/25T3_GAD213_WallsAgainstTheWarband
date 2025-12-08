using UnityEngine;

public class IceProjectile : MonoBehaviour
{
    public float speed = 8f;

    [Header("Slow")]
    public float slowMultiplier = 0.5f;
    public float slowDuration = 2f;

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
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.3f)
        {
            target.TakeDamage(damage);

            EnemyMovement move = target.GetComponent<EnemyMovement>();
            if (move != null)
                move.ApplySlow(slowMultiplier, slowDuration);

            Destroy(gameObject);
        }
    }
}
