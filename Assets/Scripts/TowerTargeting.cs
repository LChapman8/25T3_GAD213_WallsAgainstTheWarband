using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class TowerTargeting : MonoBehaviour
{
    [Header("Targeting Settings")]
    public float range = 6f;               // Attack radius
    public LayerMask enemyLayer;

    [Header("Runtime")]
    public MinionStats currentTarget;

    void Update()
    {
        AcquireTarget();
    }

    void AcquireTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        if (hits.Length == 0)
        {
            currentTarget = null;
            return;
        }

        // pick the enemy with the highest progressDistance
        MinionStats best = null;
        float bestProgress = -1f;

        foreach (var h in hits)
        {
            MinionStats ms = h.GetComponent<MinionStats>();
            if (ms != null && ms.currentHealth > 0)
            {
                if (ms.progressDistance > bestProgress)
                {
                    bestProgress = ms.progressDistance;
                    best = ms;
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
