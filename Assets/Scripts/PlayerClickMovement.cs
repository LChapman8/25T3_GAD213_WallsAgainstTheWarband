using UnityEngine;

public class PlayerClickMovement : MonoBehaviour
{
    [Header("Camera & Movement")]
    public Camera mainCamera;      // Assign your main camera
    public float speed = 5f;       // Movement speed
    public Animator animator;      // Drag your Animator in Inspector

    [Header("Fixed Height")]
    public float fixedY = 0f;      // Y position the player should always stay at

    [Header("Move VFX")]
    public GameObject clickVFXPrefab;   // Prefab for normal move click indicator
    public GameObject blockedVFXPrefab; // Prefab for forbidden click indicator
    public LayerMask forbiddenLayer;    // Layer for forbidden zones

    [Header("Audio")]
    public AudioSource runAudio;         // Assign AudioSource with running clip

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        targetPosition = transform.position;

        // Set the initial Y position
        Vector3 pos = transform.position;
        pos.y = fixedY;
        transform.position = pos;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        HandleMovement();
        HandleAnimator();
        HandleAudio();
    }

    private void HandleInput()
    {
        // Right-click sets target
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check for forbidden zones
                Collider[] hits = Physics.OverlapSphere(hit.point, 0.1f, forbiddenLayer);
                if (hits.Length > 0)
                {
                    // Spawn blocked VFX
                    if (blockedVFXPrefab != null)
                    {
                        GameObject vfx = Instantiate(blockedVFXPrefab, hit.point, Quaternion.identity);
                        Destroy(vfx, 1f);
                    }
                    return; // Don't move
                }

                // Normal move
                targetPosition = hit.point;
                targetPosition.y = fixedY; // Keep target at fixed Y
                isMoving = true;

                // Spawn click VFX
                if (clickVFXPrefab != null)
                {
                    GameObject vfx = Instantiate(clickVFXPrefab, hit.point, Quaternion.identity);
                    Destroy(vfx, 1f);
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (isMoving)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Ensure horizontal movement only
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
        }

        // Keep Y at fixed level every frame
        Vector3 currentPos = transform.position;
        currentPos.y = fixedY;
        transform.position = currentPos;
    }

    private void HandleAnimator()
    {
        if (animator != null)
            animator.SetBool("IsMoving", isMoving);
    }

    private void HandleAudio()
    {
        if (runAudio != null)
        {
            if (isMoving && !runAudio.isPlaying)
                runAudio.Play();
            else if (!isMoving && runAudio.isPlaying)
                runAudio.Stop();
        }
    }
}
