using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    public float speed = 12f;

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

        // Move towards target
        Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Reached target?
        if (Vector3.Distance(transform.position, target.transform.position) < 0.3f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
