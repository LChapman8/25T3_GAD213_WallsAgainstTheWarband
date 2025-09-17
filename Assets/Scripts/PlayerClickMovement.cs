using UnityEngine;

public class PlayerClickMovement : MonoBehaviour
{
    public Camera mainCamera;      // Assign your main camera
    public float speed = 5f;       // Movement speed
    public Terrain terrain;        // Assign your terrain in Inspector

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (terrain == null)
            terrain = Terrain.activeTerrain;

        targetPosition = transform.position;
    }

    void Update()
    {
        // Right-click sets target
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        // Move toward target
        if (isMoving)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Horizontal movement only
            float distanceThisFrame = speed * Time.deltaTime;

            if (direction.magnitude <= distanceThisFrame)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
            else
            {
                transform.Translate(direction.normalized * distanceThisFrame, Space.World);
                if (direction != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direction);
            }

            // Keep player on terrain
            float terrainHeight = terrain.SampleHeight(transform.position) + terrain.GetPosition().y;
            Vector3 pos = transform.position;
            pos.y = terrainHeight;
            transform.position = pos;
        }
    }
}
