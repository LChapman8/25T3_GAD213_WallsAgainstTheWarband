using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab; // Goblin prefab
    public Transform spawnPoint;
    public Transform waypointsParent; // Parent object of all waypoints
    public float spawnInterval = 2f; // Seconds between spawns
    public int totalEnemies = 10;
    public Terrain terrain; // Assign terrain if you want enemies to snap immediately

    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        Transform[] waypoints = GetWaypoints();

        for (int i = 0; i < totalEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            movement.waypoints = waypoints;
            movement.terrain = terrain;

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Transform[] GetWaypoints()
    {
        Transform[] points = new Transform[waypointsParent.childCount];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = waypointsParent.GetChild(i);
        }
        return points;
    }
}
