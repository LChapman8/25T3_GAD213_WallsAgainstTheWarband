using UnityEngine;

public class CannonTowerAttack : MonoBehaviour
{
    [Header("Tower Stats")]
    public float range = 8f;
    public float fireRate = 0.5f; // slower than arrow
    public int damage = 20;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Audio")]
    public AudioClip shootSound;

    private float fireCooldown = 0f;
    public MinionStats currentTarget;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (currentTarget == null || !IsTargetValid(currentTarget))
            currentTarget = GetBestTarget();

        if (currentTarget != null && fireCooldown <= 0f)
        {
            FireProjectile(currentTarget);
            fireCooldown = 1f / fireRate;
        }
    }

    bool IsTargetValid(MinionStats enemy)
    {
        if (enemy == null || enemy.currentHealth <= 0)
            return false;

        return Vector3.Distance(transform.position, enemy.transform.position) <= range;
    }

    MinionStats GetBestTarget()
    {
        MinionStats[] allMinions =
            Object.FindObjectsByType<MinionStats>(FindObjectsSortMode.None);

        MinionStats best = null;
        float bestProgress = -1f;

        foreach (var m in allMinions)
        {
            float dist = Vector3.Distance(transform.position, m.transform.position);
            if (dist > range) continue;

            EnemyMovement move = m.GetComponent<EnemyMovement>();
            if (move == null) continue;

            float progress = move.CurrentProgressValue();
            if (progress > bestProgress)
            {
                bestProgress = progress;
                best = m;
            }
        }

        return best;
    }

    void FireProjectile(MinionStats target)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        CannonProjectile cannon = proj.GetComponent<CannonProjectile>();
        cannon.Initialize(target, damage);

        if (shootSound != null)
            PlaySoundAtPosition.PlayClip(shootSound, firePoint.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
