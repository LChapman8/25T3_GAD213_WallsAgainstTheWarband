using System.Collections;
using UnityEngine;

public class TowerPlacementManager : MonoBehaviour
{
    public static TowerPlacementManager Instance;

    [Header("References")]
    public LayerMask buildableLayer;
    public LayerMask blockedLayer;
    public GameObject blockedVFXPrefab;
    public AudioSource buildAudio;
    public float buildDelay = 2f;

    [Header("Placement Settings")]
    public float placementYOffset = 0.5f;
    public Vector3 rotationOffset = new Vector3(90f, 0f, 0f);

    [Header("Construction VFX")]
    public GameObject constructionVFXPrefab;

    [Header("Player Gold")]
    public GoldManager goldManager;

    private GameObject currentGhost;
    private GameObject selectedTowerPrefab;
    private int selectedTowerCost;
    private bool isPlacing = false;

    private PlayerClickMovement player;

    private void Awake()
    {
        Instance = this;
        player = Object.FindAnyObjectByType<PlayerClickMovement>();
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
        currentGhost.transform.rotation = Quaternion.Euler(rotationOffset);
        isPlacing = true;
    }

    private void HandleGhostFollowMouse()
    {
        if (currentGhost == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 pos = hit.point + Vector3.up * placementYOffset;
            currentGhost.transform.position = pos;
            currentGhost.transform.rotation = Quaternion.Euler(rotationOffset);

            bool validPlacement = Physics.CheckSphere(hit.point, 0.5f, buildableLayer.value)
                                  && !Physics.CheckSphere(hit.point, 0.5f, blockedLayer.value);

            Renderer[] rends = currentGhost.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in rends)
            {
                foreach (Material mat in r.materials)
                {
                    mat.color = validPlacement
                        ? new Color(0f, 1f, 0f, 0.3f)
                        : new Color(1f, 0f, 0f, 0.3f);
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
                Vector3 pos = hit.point + Vector3.up * placementYOffset;

                bool validPlacement = Physics.CheckSphere(hit.point, 0.5f, buildableLayer.value)
                                      && !Physics.CheckSphere(hit.point, 0.5f, blockedLayer.value);

                if (!validPlacement)
                {
                    if (blockedVFXPrefab)
                    {
                        GameObject vfx = Instantiate(blockedVFXPrefab, pos, Quaternion.identity);
                        Destroy(vfx, 1.5f);
                    }
                    return;
                }

                // Have player walk to the build spot, then build when they arrive
                if (player != null)
                {
                    // keep the ghost visible while player walks
                    currentGhost.transform.position = pos;
                    currentGhost.transform.rotation = Quaternion.Euler(rotationOffset);

                    // Move player to the spot, with callback to start building
                    player.MoveToPoint(pos, () =>
                    {
                        // ensure player is close enough before building
                        if (Vector3.Distance(player.transform.position, pos) <= 2.5f)
                        {
                            StartCoroutine(BuildTower(pos));
                        }
                    });

                    // exit placing mode until build coroutine runs (ghost remains)
                    isPlacing = false;
                }
                else
                {
                    StartCoroutine(BuildTower(pos));
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
            CancelPlacement();
    }

    private IEnumerator BuildTower(Vector3 position)
    {
        // currentGhost remains visible at position while we build
        if (currentGhost != null)
        {
            currentGhost.transform.position = position;
            currentGhost.transform.rotation = Quaternion.Euler(rotationOffset);

            Vector3 originalScale = currentGhost.transform.localScale;
            currentGhost.transform.localScale = originalScale * 0.5f; // start small
            SetGhostColor(currentGhost, new Color(1f, 1f, 1f, 0.4f));

            if (constructionVFXPrefab != null)
            {
                GameObject vfx = Instantiate(constructionVFXPrefab, position, Quaternion.identity);
                Destroy(vfx, 2f);
            }
        }

        // Spend gold
        if (goldManager != null)
        {
            bool success = goldManager.SpendGold(selectedTowerCost);
            if (!success)
            {
                Debug.LogWarning("Failed to spend gold!");
                yield break;
            }
        }

        // Play build audio
        if (buildAudio != null)
            buildAudio.Play();

        // Smoothly scale ghost to full size over buildDelay
        float elapsed = 0f;
        Vector3 startScale = currentGhost != null ? currentGhost.transform.localScale : Vector3.one * 0.5f;
        Vector3 targetScale = startScale * 2f;
        while (elapsed < buildDelay)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / buildDelay);
            if (currentGhost != null)
                currentGhost.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        Instantiate(selectedTowerPrefab, position, Quaternion.Euler(rotationOffset));

        if (currentGhost != null)
            Destroy(currentGhost);

        selectedTowerPrefab = null;
        selectedTowerCost = 0;

        // allow placing again after build finished
        isPlacing = true;
    }

    private void SetGhostColor(GameObject ghost, Color color)
    {
        Renderer[] rends = ghost.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            foreach (Material mat in r.materials)
            {
                mat.color = color;
            }
        }
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
