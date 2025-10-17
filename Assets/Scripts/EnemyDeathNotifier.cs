using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    private EnemySpawner spawner;

    // Called by spawner immediately after Instantiate
    public void Initialize(EnemySpawner spawnerReference)
    {
        spawner = spawnerReference;
    }

    private void OnDestroy()
    {
        // Notify only if spawner still exists
        if (spawner != null)
            spawner.OnEnemyDied();
    }
}
