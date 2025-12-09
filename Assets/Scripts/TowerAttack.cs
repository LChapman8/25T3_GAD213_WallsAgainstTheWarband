using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    public float range = 10f;
    public float fireRate = 1f;
    public int damage = 10;

    public GameObject projectilePrefab;
    public Transform firePoint;
    public AudioClip shootSound;

    private float cooldown;
    private IEnemy currentTarget;

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (currentTarget == null || !IsTargetValid(currentTarget))
            currentTarget = FindBestTarget();

        if (currentTarget != null && cooldown <= 0f)
        {
            FireProjectile(currentTarget);
            cooldown = 1f / fireRate;
        }
    }

    bool IsTargetValid(IEnemy enemy)
    {
        if (enemy == null || enemy.CurrentHealth <= 0)
            return false;

        return Vector3.Distance(transform.position, enemy.Transform.position) <= range;
    }

    IEnemy FindBestTarget()
    {
        IEnemy best = null;
        float bestProgress = float.MinValue;

        MonoBehaviour[] all =
            Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        foreach (var mb in all)
        {
            if (mb is IEnemy enemy)
            {
                float dist = Vector3.Distance(transform.position, enemy.Transform.position);
                if (dist > range) continue;

                if (enemy.Progress > bestProgress)
                {
                    bestProgress = enemy.Progress;
                    best = enemy;
                }
            }
        }

        return best;
    }

    void FireProjectile(IEnemy target)
    {
        GameObject proj =
            Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        if (proj.TryGetComponent(out ArrowProjectile arrow))
            arrow.Initialize(target, damage);
        else if (proj.TryGetComponent(out IceProjectile ice))
            ice.Initialize(target, damage);
        else if (proj.TryGetComponent(out CannonProjectile cannon))
            cannon.Initialize(target, damage);

        if (shootSound != null)
            PlaySoundAtPosition.PlayClip(shootSound, firePoint.position);
    }
}
