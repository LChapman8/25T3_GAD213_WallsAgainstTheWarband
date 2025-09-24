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
    public float minZoom = 10f;
    public float maxZoom = 30f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
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

        // Zoom: only move the camera along its local forward axis using localPosition
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log("scroll value is " + scroll);
        if (scroll != 0f)
        {
            Vector3 zoom = transform.forward * scroll * scrollSpeed;
            float nextHeight = transform.position.y - zoom.y; // keep Y consistent
            // if zoom is below minimum, set zoom to minimum
            // if zoom is above max, set zoom to maximum 
            if (nextHeight >= minZoom && nextHeight <= maxZoom)
            {
                transform.position += zoom;
            }
        }
    }
}
