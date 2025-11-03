using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    [Header("Input Cooldown")]
    [Tooltip("Minimum time (in seconds) between movement clicks.")]
    public float clickCooldown = 1.5f;
    private float lastClickTime = -999f;

    [Header("Movement Limits")]
    [Tooltip("Max distance the player can move from their start position.")]
    public float maxMoveRadius = 25f;
    private Vector3 startPosition;

    [Header("Stopping Distance")]
    [Tooltip("Distance before the final target position to stop.")]
    public float stopOffset = 0.5f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private int towerLayer;

    private Action onArriveCallback = null;
    private bool followingCallbackPath = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        towerLayer = LayerMask.NameToLayer("Tower");

        targetPosition = transform.position;
        startPosition = transform.position;

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

        // Left-click handling (for hiding UI)
        if (Input.GetMouseButtonDown(0))
        {
            // Ignore clicks on UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Only hide menu when clicking something that is NOT a tower
                if (hit.collider.GetComponent<TowerClickable>() == null)
                    TowerMenuUI.Instance.Hide();
            }
            else
            {
                TowerMenuUI.Instance.Hide();
            }
        }
    }

    private void HandleInput()
    {
        // Ignore clicks on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Time.time - lastClickTime < clickCooldown)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            lastClickTime = Time.time;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Ignore tower clicks (those open the UI)
                if (hit.collider.GetComponent<TowerClickable>() != null)
                    return;

                Vector3 clickPoint = hit.point;
                clickPoint.y = fixedY;

                Vector3 lookDir = clickPoint - transform.position;
                lookDir.y = 0;
                if (lookDir.sqrMagnitude > 0.01f)
                    transform.rotation = Quaternion.LookRotation(lookDir);

                // Stay within allowed radius
                if (Vector3.Distance(startPosition, clickPoint) > maxMoveRadius)
                {
                    SpawnBlockedVFX(clickPoint);
                    Debug.Log("Target out of range");
                    return;
                }

                if (hit.collider != null && hit.collider.gameObject.layer == towerLayer)
                {
                    SpawnBlockedVFX(clickPoint);
                    return;
                }

                // If clicked directly in forbidden zone
                if (Physics.CheckSphere(clickPoint, 0.1f, forbiddenLayer.value))
                {
                    SpawnBlockedVFX(clickPoint);
                    return;
                }

                Vector3 dirToClick = clickPoint - transform.position;
                float distToClick = dirToClick.magnitude;
                bool blockedByTower = Physics.Raycast(transform.position + Vector3.up * 0.1f, dirToClick.normalized, distToClick, 1 << towerLayer);
                bool blockedByForbidden = Physics.Raycast(transform.position + Vector3.up * 0.1f, dirToClick.normalized, distToClick, forbiddenLayer.value);

                if (blockedByTower)
                {
                    SpawnBlockedVFX(clickPoint);
                    return;
                }

                // Handle bridges if forbidden between player and target
                if (blockedByForbidden)
                {
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
                        List<Vector3> orderedWaypoints = usableBridge.GetOrderedWaypoints(transform.position);

                        bool bridgeBlocked = false;
                        Vector3 lastPos = transform.position;
                        foreach (Vector3 wp in orderedWaypoints)
                        {
                            Vector3 segment = wp - lastPos;
                            if (segment.sqrMagnitude <= 0.0001f)
                            {
                                lastPos = wp;
                                continue;
                            }

                            if (Physics.Raycast(lastPos + Vector3.up * 0.1f, segment.normalized, segment.magnitude, forbiddenLayer.value) ||
                                Physics.Raycast(lastPos + Vector3.up * 0.1f, segment.normalized, segment.magnitude, 1 << towerLayer))
                            {
                                bridgeBlocked = true;
                                break;
                            }
                            lastPos = wp;
                        }

                        if (bridgeBlocked)
                        {
                            SpawnBlockedVFX(clickPoint);
                            return;
                        }

                        pathPoints.Clear();
                        foreach (Vector3 wp in orderedWaypoints)
                            pathPoints.Enqueue(AdjustStopPoint(wp));

                        pathPoints.Enqueue(AdjustStopPoint(clickPoint));
                        targetPosition = pathPoints.Dequeue();
                        isMoving = true;
                        SpawnClickVFX(clickPoint);
                        return;
                    }

                    SpawnBlockedVFX(clickPoint);
                    return;
                }

                // Direct move
                pathPoints.Clear();
                targetPosition = AdjustStopPoint(clickPoint);
                isMoving = true;
                SpawnClickVFX(clickPoint);
            }
        }
    }

    private Vector3 AdjustStopPoint(Vector3 destination)
    {
        Vector3 dir = destination - transform.position;
        dir.y = 0;
        float distance = dir.magnitude;

        if (distance > stopOffset)
        {
            dir.Normalize();
            destination -= dir * stopOffset;
        }

        destination.y = fixedY;
        return destination;
    }

    private void SpawnClickVFX(Vector3 position)
    {
        if (clickVFXPrefab != null)
        {
            GameObject vfx = Instantiate(clickVFXPrefab, position, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }

    private void SpawnBlockedVFX(Vector3 position)
    {
        if (blockedVFXPrefab != null)
        {
            GameObject blockedVfx = Instantiate(blockedVFXPrefab, position, Quaternion.identity);
            Destroy(blockedVfx, 1f);
        }
    }

    public void MoveToPoint(Vector3 targetPoint, Action onArrive = null)
    {
        pathPoints.Clear();
        onArriveCallback = onArrive;
        followingCallbackPath = (onArriveCallback != null);

        targetPoint = AdjustStopPoint(targetPoint);

        Vector3 dirToTarget = targetPoint - transform.position;
        float dist = dirToTarget.magnitude;
        bool blockedByForbidden = false;
        if (dist > 0.001f)
            blockedByForbidden = Physics.Raycast(transform.position + Vector3.up * 0.1f, dirToTarget.normalized, dist, forbiddenLayer.value);

        if (blockedByForbidden)
        {
            Bridge usableBridge = null;
            float minDist = Mathf.Infinity;

            foreach (Bridge b in bridges)
            {
                Vector3 bridgeEnd = b.GetClosestEnd(targetPoint);
                float d = Vector3.Distance(targetPoint, bridgeEnd);
                if (d < minDist)
                {
                    minDist = d;
                    usableBridge = b;
                }
            }

            if (usableBridge != null)
            {
                List<Vector3> orderedWaypoints = usableBridge.GetOrderedWaypoints(transform.position);
                foreach (Vector3 wp in orderedWaypoints)
                    pathPoints.Enqueue(AdjustStopPoint(wp));

                pathPoints.Enqueue(AdjustStopPoint(targetPoint));
                targetPosition = pathPoints.Dequeue();
                isMoving = true;
                return;
            }
        }

        targetPosition = AdjustStopPoint(targetPoint);
        isMoving = true;
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
            {
                targetPosition = pathPoints.Dequeue();
            }
            else
            {
                isMoving = false;

                if (followingCallbackPath && onArriveCallback != null)
                {
                    Action cb = onArriveCallback;
                    onArriveCallback = null;
                    followingCallbackPath = false;
                    cb.Invoke();
                }

                lastClickTime = -999f;
            }
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

[Serializable]
public class Bridge
{
    public Transform[] waypoints;

    public Vector3 GetClosestEnd(Vector3 point)
    {
        if (waypoints == null || waypoints.Length == 0) return Vector3.zero;
        float distStart = Vector3.Distance(point, waypoints[0].position);
        float distEnd = Vector3.Distance(point, waypoints[waypoints.Length - 1].position);
        return distStart < distEnd ? waypoints[0].position : waypoints[waypoints.Length - 1].position;
    }

    public List<Vector3> GetOrderedWaypoints(Vector3 fromPosition)
    {
        List<Vector3> ordered = new List<Vector3>();
        if (waypoints == null || waypoints.Length == 0) return ordered;

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
