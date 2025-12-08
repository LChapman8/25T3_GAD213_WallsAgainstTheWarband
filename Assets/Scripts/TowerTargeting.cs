using UnityEngine;
using System.Linq;

public class TowerTargeting : MonoBehaviour
{
    [Header("Targeting Settings")]
    public float range = 6f;
    public LayerMask enemyLayer;

    [Header("Runtime")]
    public IEnemy currentTarget;

    void Update()
    {
        AcquireTarget();
    }

    void AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        IEnemy best = null;
        float bestProgress = -1f;

        foreach (var h in hits)
        {
            IEnemy enemy = h.GetComponent<MonoBehaviour>() as IEnemy;
            if (enemy != null && enemy.CurrentHealth > 0)
            {
                if (enemy.Progress > bestProgress)
                {
                    bestProgress = enemy.Progress;
                    best = enemy;
                }
            }
        }

        currentTarget = best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
