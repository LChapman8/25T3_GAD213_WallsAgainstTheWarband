using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints; // Set in Inspector
    public float speed = 3f;
    public float rotationOffsetY = 0f; // Adjust if goblin faces wrong way
    public Terrain terrain; // Assign your terrain in Inspector

    private int currentWaypoint = 0;

    void Start()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Keep horizontal movement only

        // Move towards waypoint
        transform.position += direction.normalized * speed * Time.deltaTime;

        // Rotate to face waypoint with offset
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation * Quaternion.Euler(0f, rotationOffsetY, 0f);
        }

        // Snap Y position to terrain height
        if (terrain != null)
        {
            float terrainHeight = terrain.SampleHeight(transform.position) + terrain.GetPosition().y;
            Vector3 pos = transform.position;
            pos.y = terrainHeight;
            transform.position = pos;
        }

        // Check if reached waypoint
        if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                             new Vector3(target.position.x, 0, target.position.z)) < 0.1f)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                ReachDestination();
            }
        }
    }

    void ReachDestination()
    {
        // TODO: deal damage to player base
        Destroy(gameObject);
    }
}
