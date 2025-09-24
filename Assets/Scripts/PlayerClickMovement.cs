using UnityEngine;

public class PlayerClickMovement : MonoBehaviour
{
    public Camera mainCamera;      // Assign your main camera
    public float speed = 5f;       // Movement speed
    public Terrain terrain;        // Assign your terrain in Inspector
    public Animator animator;      // Drag your Animator in Inspector

    [Header("Terrain Snapping")]
    public bool snapToTerrain = false; // Toggle snapping on/off
    public float snapOffset = 0.0f;    // Offset above terrain (adjust if sinking)

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (terrain == null)
            terrain = Terrain.activeTerrain;

        targetPosition = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();
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

            // Snap to terrain if enabled
            if (snapToTerrain && terrain != null)
            {
                float terrainHeight = terrain.SampleHeight(transform.position) + terrain.GetPosition().y + snapOffset;
                Vector3 pos = transform.position;
                pos.y = terrainHeight;
                transform.position = pos;
            }
        }

        // Feed Animator
        float currentSpeed = isMoving ? speed : 0f;
        if (animator != null)
            animator.SetFloat("Speed", currentSpeed);
    }

    // Call this when you want to play build animation
    public void PlayBuildAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Build");
    }
}
