using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 3f;
    public float rotationOffsetY = 0f;
    public Terrain terrain;

    private int currentWaypoint = 0;
    public float rotationSpeed = 5f; // How fast goblin turns
    public float stoppingDistance = 0.2f; // Start slowing near waypoint

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
        direction.y = 0;

        float distance = direction.magnitude;

        // Move towards waypoint
        if (distance > stoppingDistance)
        {
            Vector3 move = direction.normalized * speed * Time.deltaTime;
            transform.position += move;
        }
        else
        {
            // Reached waypoint
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                ReachDestination();
                return;
            }
        }

        // Smooth rotation
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, rotationOffsetY, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Keep on terrain
        if (terrain != null)
        {
            float terrainHeight = terrain.SampleHeight(transform.position) + terrain.GetPosition().y;
            Vector3 pos = transform.position;
            pos.y = terrainHeight;
            transform.position = pos;
        }
    }

    void ReachDestination()
    {
        Destroy(gameObject);
    }

    public float CurrentProgressValue()
    {
        // Higher value = more progressed along the path
        if (currentWaypoint >= waypoints.Length) return float.MaxValue;

        Transform target = waypoints[currentWaypoint];
        float distToNext = Vector3.Distance(transform.position, target.position);

        // Lower distance = more progress, so invert it
        return currentWaypoint * 1000f - distToNext;
    }

}

