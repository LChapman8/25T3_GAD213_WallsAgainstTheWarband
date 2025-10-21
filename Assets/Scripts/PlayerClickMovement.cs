using UnityEngine;
using System.Collections.Generic;

public class PlayerClickMovement : MonoBehaviour
{
    [Header("Camera & Movement")]
    public Camera mainCamera;
    public float speed = 5f;
    public Animator animator;

    [Header("Fixed Height")]
    public float fixedY = 0f;

    [Header("Move VFX")]
    public GameObject clickVFXPrefab;
    public GameObject blockedVFXPrefab;
    public LayerMask forbiddenLayer;

    [Header("Audio")]
    public AudioSource runAudio;

    [Header("Bridge Setup")]
    public Bridge[] bridges; 

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        targetPosition = transform.position;
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
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 clickPoint = hit.point;
                clickPoint.y = fixedY;

                // Check if click is in a forbidden zone
                if (Physics.CheckSphere(clickPoint, 0.1f, forbiddenLayer))
                {
                    if (blockedVFXPrefab != null)
                    {
                        GameObject blockedVfx = Instantiate(blockedVFXPrefab, clickPoint, Quaternion.identity);
                        Destroy(blockedVfx, 1f);
                    }
                    return; // Don't move at all
                }

                // Spawn normal click VFX
                if (clickVFXPrefab != null)
                {
                    GameObject vfx = Instantiate(clickVFXPrefab, clickPoint, Quaternion.identity);
                    Destroy(vfx, 1f);
                }

                // Check if path intersects forbidden zones
                bool blockedPath = Physics.Linecast(transform.position, clickPoint, forbiddenLayer);
                if (!blockedPath)
                {
                    // Direct path is valid
                    pathPoints.Clear();
                    targetPosition = clickPoint;
                    isMoving = true;
                    return;
                }

                // Path is blocked, check for bridges
                Bridge usableBridge = null;
                float minDist = Mathf.Infinity;
                foreach (Bridge b in bridges)
                {
                    Vector3 bridgeEnd = b.GetClosestEnd(clickPoint);
                    float dist = Vector3.Distance(clickPoint, bridgeEnd);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        usableBridge = b;
                    }
                }

                if (usableBridge != null)
                {
                    // Use bridge waypoints
                    List<Vector3> orderedWaypoints = usableBridge.GetOrderedWaypoints(transform.position);

                    // Verify the bridge path does not cross forbidden zones
                    bool bridgeBlocked = false;
                    Vector3 lastPos = transform.position;
                    foreach (Vector3 wp in orderedWaypoints)
                    {
                        if (Physics.Linecast(lastPos, wp, forbiddenLayer))
                        {
                            bridgeBlocked = true;
                            break;
                        }
                        lastPos = wp;
                    }

                    if (bridgeBlocked)
                    {
                        // Bridge path is blocked
                        if (blockedVFXPrefab != null)
                        {
                            GameObject blockedVfx = Instantiate(blockedVFXPrefab, clickPoint, Quaternion.identity);
                            Destroy(blockedVfx, 1f);
                        }
                        return;
                    }

                    // Bridge is safe, enqueue all waypoints
                    pathPoints.Clear();
                    foreach (Vector3 wp in orderedWaypoints)
                        pathPoints.Enqueue(wp);

                    pathPoints.Enqueue(clickPoint); // final target
                    targetPosition = pathPoints.Dequeue();
                    isMoving = true;
                }
                else
                {
                    // No bridge possible, spawn blocked VFX
                    if (blockedVFXPrefab != null)
                    {
                        GameObject blockedVfx = Instantiate(blockedVFXPrefab, clickPoint, Quaternion.identity);
                        Destroy(blockedVfx, 1f);
                    }
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (!isMoving) return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            transform.position = targetPosition;

            if (pathPoints.Count > 0)
                targetPosition = pathPoints.Dequeue();
            else
                isMoving = false;
        }
        else
        {
            transform.Translate(direction.normalized * distanceThisFrame, Space.World);
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);
        }

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

[System.Serializable]
public class Bridge
{
    public Transform[] waypoints;

    public Vector3 GetClosestEnd(Vector3 point)
    {
        if (waypoints.Length == 0) return Vector3.zero;
        float distStart = Vector3.Distance(point, waypoints[0].position);
        float distEnd = Vector3.Distance(point, waypoints[waypoints.Length - 1].position);
        return distStart < distEnd ? waypoints[0].position : waypoints[waypoints.Length - 1].position;
    }

    public List<Vector3> GetOrderedWaypoints(Vector3 fromPosition)
    {
        List<Vector3> ordered = new List<Vector3>();
        if (waypoints.Length == 0) return ordered;

        float distStart = Vector3.Distance(fromPosition, waypoints[0].position);
        float distEnd = Vector3.Distance(fromPosition, waypoints[waypoints.Length - 1].position);

        if (distStart <= distEnd)
        {
            foreach (Transform wp in waypoints)
                ordered.Add(new Vector3(wp.position.x, fromPosition.y, wp.position.z));
        }
        else
        {
            for (int i = waypoints.Length - 1; i >= 0; i--)
                ordered.Add(new Vector3(waypoints[i].position.x, fromPosition.y, waypoints[i].position.z));
        }

        return ordered;
    }
}
