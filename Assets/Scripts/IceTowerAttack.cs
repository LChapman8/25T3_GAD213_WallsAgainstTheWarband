using UnityEngine;

public class IceTowerAttack : MonoBehaviour
{
    [Header("Stats")]
    public float range = 9f;
    public float fireRate = 0.8f;
    public int damage = 10;

    [Header("Projectile")]
    public GameObject iceProjectilePrefab;
    public Transform firePoint;

    [Header("Audio")]
    public AudioClip shootSound;

    private float cooldown;
    private MinionStats currentTarget;

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (currentTarget == null || !IsTargetValid(currentTarget))
            currentTarget = FindMostProgressedTarget();

        if (currentTarget != null && cooldown <= 0f)
        {
            FireAtTarget(currentTarget);
            cooldown = 1f / fireRate;
        }
    }

    void FireAtTarget(MinionStats target)
    {
        GameObject proj =
            Instantiate(iceProjectilePrefab, firePoint.position, Quaternion.identity);

        IceProjectile ice = proj.GetComponent<IceProjectile>();
        ice.Initialize(target, damage);

        if (shootSound != null)
            PlaySoundAtPosition.PlayClip(shootSound, firePoint.position);
    }

    bool IsTargetValid(MinionStats enemy)
    {
        if (enemy == null || enemy.currentHealth <= 0)
            return false;

        return Vector3.Distance(transform.position, enemy.transform.position) <= range;
    }

    MinionStats FindMostProgressedTarget()
    {
        MinionStats[] enemies =
            Object.FindObjectsByType<MinionStats>(FindObjectsSortMode.None);

        MinionStats best = null;
        float bestProgress = float.MinValue;

        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist > range) continue;

            EnemyMovement move = e.GetComponent<EnemyMovement>();
            if (move == null) continue;

            float progress = move.CurrentProgressValue();
            if (progress > bestProgress)
            {
                bestProgress = progress;
                best = e;
            }
        }

        return best;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
