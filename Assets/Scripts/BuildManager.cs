using UnityEngine;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("References")]
    public LayerMask buildZoneLayer; // Only allow placement on these layers
    public LayerMask towerLayer;     // Prevent placement on existing towers

    [Header("Ghost Settings")]
    public Material validMaterial;
    public Material invalidMaterial;
    public float placementYOffset = 0.1f;

    private GameObject currentGhost;
    private GameObject towerToBuild;
    private Renderer[] ghostRenderers;
    private bool canPlace = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (currentGhost == null) return;

        // Don't place when hovering over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        UpdateGhostPosition();

        if (Input.GetMouseButtonDown(0))
        {
            if (canPlace)
            {
                PlaceTower();
            }
            else
            {
                Debug.Log("Invalid build position");
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelBuildMode();
        }
    }

    private void UpdateGhostPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            currentGhost.transform.position = hit.point + Vector3.up * placementYOffset;

            bool hitBuildZone = (buildZoneLayer.value & (1 << hit.collider.gameObject.layer)) != 0;
            bool overlapsTower = Physics.CheckSphere(hit.point, 1f, towerLayer);

            canPlace = hitBuildZone && !overlapsTower;

            // Update material color
            foreach (var r in ghostRenderers)
                r.material = canPlace ? validMaterial : invalidMaterial;
        }
    }

    public void EnterBuildMode(GameObject towerPrefab)
    {
        if (currentGhost != null)
            Destroy(currentGhost);

        towerToBuild = towerPrefab;
        currentGhost = Instantiate(towerPrefab);
        ghostRenderers = currentGhost.GetComponentsInChildren<Renderer>();

        // Make ghost semi-transparent
        foreach (var r in ghostRenderers)
        {
            r.material = validMaterial;
            Color c = r.material.color;
            c.a = 0.3f;
            r.material.color = c;
        }
    }

    private void PlaceTower()
    {
        Vector3 pos = currentGhost.transform.position;
        Instantiate(towerToBuild, pos, currentGhost.transform.rotation);
        CancelBuildMode();
    }

    private void CancelBuildMode()
    {
        if (currentGhost != null)
            Destroy(currentGhost);

        currentGhost = null;
        towerToBuild = null;
        canPlace = false;
    }
}
