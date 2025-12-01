using UnityEngine;
using System.Collections.Generic;

public class TowerAttack : MonoBehaviour
{
    [Header("Tower Stats")]
    public float range = 10f;
    public float fireRate = 1f;
    public int damage = 10;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Audio")]
    public AudioClip shootSound;   

    private float fireCooldown = 0f;
    private MinionStats currentTarget;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        // If no target or target left range/died, find new one
        if (currentTarget == null || !IsTargetValid(currentTarget))
        {
            currentTarget = GetBestTarget();
        }

        if (currentTarget != null && fireCooldown <= 0f)
        {
            FireProjectile(currentTarget);
            fireCooldown = 1f / fireRate;
        }
    }

    bool IsTargetValid(MinionStats enemy)
    {
        if (enemy == null) return false;
        if (enemy.currentHealth <= 0) return false;

        float dist = Vector3.Distance(transform.position, enemy.transform.position);
        return dist <= range;
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
        // Spawn the projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        ArrowProjectile arrow = proj.GetComponent<ArrowProjectile>();
        arrow.Initialize(target, damage);

        // Play shot sound 
        if (shootSound != null)
            PlaySoundAtPosition.PlayClip(shootSound, firePoint.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
