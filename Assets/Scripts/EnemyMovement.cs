using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] waypoints;
    public float baseSpeed = 3f;
    private float currentSpeed;

    public float rotationOffsetY = 0f;
    public Terrain terrain;

    private int currentWaypoint = 0;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.2f;

    [Header("Ice Slow")]
    public Material iceMaterial;
    private Material[] originalMaterials;
    private Renderer[] renderers;
    private bool isSlowed = false;
    private float slowTimer = 0f;

    [HideInInspector] public float progressDistance = 0f;

    void Start()
    {
        currentSpeed = baseSpeed;

        renderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalMaterials[i] = renderers[i].material;

        if (terrain == null)
            terrain = Terrain.activeTerrain;
    }

    void Update()
    {
        HandleSlow();
        MoveAlongPath();
        UpdateProgress();
        StickToTerrain();
    }

    void MoveAlongPath()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        float distance = direction.magnitude;

        if (distance > stoppingDistance)
        {
            transform.position += direction.normalized * currentSpeed * Time.deltaTime;
        }
        else
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                HandleReachedEnd();
                return;
            }
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction) *
                                   Quaternion.Euler(0f, rotationOffsetY, 0f);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void UpdateProgress()
    {
        if (waypoints.Length == 0) return;
        Transform target = waypoints[Mathf.Min(currentWaypoint, waypoints.Length - 1)];
        float remaining = Vector3.Distance(transform.position, target.position);
        progressDistance = (currentWaypoint * 1000f) - remaining;
    }

    void HandleReachedEnd()
    {
        BossStats boss = GetComponent<BossStats>();
        if (boss != null)
        {
            boss.ReachBase();
        }
        else
        {
            BaseTriggerDamage baseTrigger = Object.FindFirstObjectByType<BaseTriggerDamage>();
            if (baseTrigger != null)
                baseTrigger.OnTriggerEnter(GetComponent<Collider>());
            Destroy(gameObject);
        }
    }

    void StickToTerrain()
    {
        if (terrain == null) return;
        float h = terrain.SampleHeight(transform.position) + terrain.GetPosition().y;
        Vector3 p = transform.position;
        p.y = h;
        transform.position = p;
    }

    void HandleSlow()
    {
        if (!isSlowed) return;

        slowTimer -= Time.deltaTime;
        if (slowTimer <= 0f)
            RemoveSlow();
    }

    public void ApplySlow(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * multiplier;
        slowTimer = duration;
        isSlowed = true;
        ApplyIceMaterial();
    }

    void ApplyIceMaterial()
    {
        if (iceMaterial == null) return;
        foreach (var r in renderers)
            r.material = iceMaterial;
    }

    void RemoveSlow()
    {
        isSlowed = false;
        currentSpeed = baseSpeed;
        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material = originalMaterials[i];
    }

    public float Progress => progressDistance;
}
