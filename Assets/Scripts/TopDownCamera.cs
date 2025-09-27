using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimitX = new Vector2(0, 80);
    public Vector2 panLimitZ = new Vector2(0, 50);

    [Header("Zoom Settings")]
    public float scrollSpeed = 20f;
    public float minZoom = 12f; // Zoom in limit (closer)
    public float maxZoom = 49f; // Zoom out limit (further)
    public float zoomSmoothness = 5f; // Higher = faster smoothing

    private Camera cam;
    private float targetZoom;

    void Start()
    {
        cam = Camera.main;
        targetZoom = transform.position.y; // start at current height
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // Keyboard movement
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 right = new Vector3(transform.right.x, 0, transform.right.z).normalized;
        pos += (right * Input.GetAxis("Horizontal") + forward * Input.GetAxis("Vertical")) * panSpeed * Time.deltaTime;

        // Mouse edge movement
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            pos += forward * panSpeed * Time.deltaTime;
        if (Input.mousePosition.y <= panBorderThickness)
            pos -= forward * panSpeed * Time.deltaTime;
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            pos += right * panSpeed * Time.deltaTime;
        if (Input.mousePosition.x <= panBorderThickness)
            pos -= right * panSpeed * Time.deltaTime;

        // Clamp pan position
        pos.x = Mathf.Clamp(pos.x, panLimitX.x, panLimitX.y);
        pos.z = Mathf.Clamp(pos.z, panLimitZ.x, panLimitZ.y);

        transform.position = pos;

        // --- Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            targetZoom -= scroll * scrollSpeed; // adjust target
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

        // Smoothly move Y toward target zoom
        Vector3 smoothPos = transform.position;
        smoothPos.y = Mathf.Lerp(transform.position.y, targetZoom, Time.deltaTime * zoomSmoothness);
        transform.position = smoothPos;
    }
}
