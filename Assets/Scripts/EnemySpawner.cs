using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public Transform waypointsParent;
    public float spawnInterval = 2f;
    public int totalEnemies = 10;
    public Terrain terrain;

    public int enemiesAlive { get; private set; } = 0;
    public int currentRound = 1; // IMPORTANT: Wave number for scaling health

    public IEnumerator SpawnEnemiesRoutine()
    {
        Transform[] waypoints = GetWaypoints();
        enemiesAlive = totalEnemies;

        for (int i = 0; i < totalEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // Initialize stats with round scaling
            MinionStats stats = enemy.GetComponent<MinionStats>();
            if (stats != null)
                stats.Initialise(currentRound);

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.waypoints = waypoints;
                movement.terrain = terrain;
            }

            EnemyDeathNotifier notifier = enemy.GetComponent<EnemyDeathNotifier>();
            if (notifier == null)
                notifier = enemy.AddComponent<EnemyDeathNotifier>();
            notifier.Initialize(this);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive < 0) enemiesAlive = 0;
    }

    private Transform[] GetWaypoints()
    {
        Transform[] points = new Transform[waypointsParent.childCount];
        for (int i = 0; i < points.Length; i++)
            points[i] = waypointsParent.GetChild(i);
        return points;
    }
}
