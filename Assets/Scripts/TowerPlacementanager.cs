using UnityEngine;
using System.Collections;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance;

    [Header("References")]
    public LayerMask buildableLayer;        // Layer for valid build zones
    public LayerMask blockedLayer;          // Layer for towers/obstacles
    public GameObject blockedVFXPrefab;     // VFX when invalid placement
    public AudioSource buildAudio;          // Play when a tower is placed
    public float buildDelay = 2f;           // Time before tower fully builds

    [Header("Player Gold")]
    public GoldManager goldManager;         // GoldManager reference

    [Header("Ghost Settings")]
    public float placementYOffset = 0.1f;   // Small hover offset for ghost tower

    private GameObject currentGhost;
    private GameObject selectedTowerPrefab;
    private int selectedTowerCost;
    private bool isPlacing = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!isPlacing) return;

        HandleGhostFollowMouse();
        HandlePlacementClick();
    }

    public void StartPlacingTower(GameObject towerPrefab, GameObject ghostPrefab, int cost)
    {
        if (goldManager != null && goldManager.CurrentGold < cost)
        {
            Debug.Log("Not enough gold!");
            return;
        }

        if (currentGhost != null)
            Destroy(currentGhost);

        selectedTowerPrefab = towerPrefab;
        selectedTowerCost = cost;

        currentGhost = Instantiate(ghostPrefab);
        currentGhost.layer = LayerMask.NameToLayer("Ignore Raycast");
        isPlacing = true;
    }

    private void HandleGhostFollowMouse()
    {
        if (currentGhost == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point;

            // Adjust Y based on ghost's renderer bounds so it sits on terrain
            Renderer ghostRenderer = currentGhost.GetComponentInChildren<Renderer>();
            if (ghostRenderer != null)
                pos.y += ghostRenderer.bounds.extents.y + placementYOffset;

            currentGhost.transform.position = pos;

            bool validPlacement = Physics.CheckSphere(pos, 0.5f, buildableLayer)
                                  && !Physics.CheckSphere(pos, 0.5f, blockedLayer);

            // Update ghost color (green = valid, red = invalid)
            Renderer[] rends = currentGhost.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
            {
                foreach (Material mat in r.materials)
                {
                    mat.color = validPlacement ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);
                }
            }
        }
    }

    private void HandlePlacementClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 pos = hit.point;

                bool validPlacement = Physics.CheckSphere(pos, 0.5f, buildableLayer)
                                      && !Physics.CheckSphere(pos, 0.5f, blockedLayer);

                if (!validPlacement)
                {
                    if (blockedVFXPrefab)
                    {
                        GameObject vfx = Instantiate(blockedVFXPrefab, pos, Quaternion.identity);
                        Destroy(vfx, 1.5f);
                    }
                    return;
                }

                StartCoroutine(BuildTower(pos));
            }
        }

        // Right click cancels building
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }
    }

    private IEnumerator BuildTower(Vector3 position)
    {
        isPlacing = false;

        if (currentGhost != null)
        {
            Destroy(currentGhost);
            currentGhost = null;
        }

        // Spend gold through GoldManager
        if (goldManager != null)
        {
            bool success = goldManager.SpendGold(selectedTowerCost);
            if (!success)
            {
                Debug.LogWarning("Failed to spend gold!");
                yield break;
            }
        }

        // Play build sound
        if (buildAudio != null)
            buildAudio.Play();

        // Simulate build delay
        yield return new WaitForSeconds(buildDelay);

        // Adjust final tower Y based on its mesh bounds
        Vector3 finalPos = position;
        if (selectedTowerPrefab != null)
        {
            Renderer towerRenderer = selectedTowerPrefab.GetComponentInChildren<Renderer>();
            if (towerRenderer != null)
                finalPos.y += towerRenderer.bounds.extents.y;
            Instantiate(selectedTowerPrefab, finalPos, Quaternion.identity);
        }

        selectedTowerPrefab = null;
        selectedTowerCost = 0;
    }

    private void CancelPlacement()
    {
        isPlacing = false;
        if (currentGhost != null)
            Destroy(currentGhost);
        selectedTowerPrefab = null;
        selectedTowerCost = 0;
    }
}
