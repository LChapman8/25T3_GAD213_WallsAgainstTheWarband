using UnityEngine;
using System.Linq;

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
            currentTarget = GetBestTarget();

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

    IEnemy GetBestTarget()
    {
        IEnemy[] allEnemies = Object.FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>().ToArray();
        IEnemy best = null;
        float bestProgress = -1f;

        foreach (var e in allEnemies)
        {
            float dist = Vector3.Distance(transform.position, e.Transform.position);
            if (dist > range) continue;

            if (e.Progress > bestProgress)
            {
                bestProgress = e.Progress;
                best = e;
            }
        }

        return best;
    }

    void FireProjectile(IEnemy target)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        if (projectilePrefab.GetComponent<ArrowProjectile>())
            proj.GetComponent<ArrowProjectile>().Initialize(target, damage);
        else if (projectilePrefab.GetComponent<IceProjectile>())
            proj.GetComponent<IceProjectile>().Initialize(target, damage);
        else if (projectilePrefab.GetComponent<CannonProjectile>())
            proj.GetComponent<CannonProjectile>().Initialize(target, damage);

        if (shootSound != null)
            PlaySoundAtPosition.PlayClip(shootSound, firePoint.position);
    }
}
