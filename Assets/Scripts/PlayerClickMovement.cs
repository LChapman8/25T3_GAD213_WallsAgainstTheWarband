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

    private Action onArriveCallback = null;
    private bool followingCallbackPath = false;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

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

        // LEFT CLICK logic
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // Check if clicking a tower
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.GetComponentInParent<TowerClickable>() != null)
                {
                    // Let the tower handle showing UI
                    return;
                }
            }

            // Otherwise, hide UI
            TowerMenuUI.Instance.Hide();
            TowerClickable.HideAllRanges();

        }
    }


    private void HandleInput()
    {
        // UI blocking click
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Movement blocked: Click was over UI.");
            return;
        }

        // Cooldown preventing click
        if (Time.time - lastClickTime < clickCooldown)
        {
            Debug.Log($"Movement blocked: Click on cooldown ({(Time.time - lastClickTime):F2}/{clickCooldown}).");
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            lastClickTime = Time.time;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("Raycast hit: " + hit.collider.name);

                Vector3 clickPoint = hit.point;
                clickPoint.y = fixedY;

                // Check distance radius
                if (Vector3.Distance(startPosition, clickPoint) > maxMoveRadius)
                {
                    Debug.Log("Movement blocked: Target out of allowed radius.");
                    SpawnBlockedVFX(clickPoint);
                    return;
                }

                // If everything is fine, move
                pathPoints.Clear();
                targetPosition = AdjustStopPoint(clickPoint);
                isMoving = true;

                Debug.Log("Movement started toward: " + targetPosition);
                SpawnClickVFX(clickPoint);
            }
            else
            {
                Debug.Log("Movement blocked: Raycast hit nothing.");
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
        targetPosition = targetPoint;
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
