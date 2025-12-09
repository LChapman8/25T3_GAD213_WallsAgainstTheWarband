using UnityEngine;
using System.Collections;

public class EndlessModeManager : MonoBehaviour
{
    [Header("Spawner Reference")]
    public EnemySpawner spawner;

    [Header("Endless Mode Settings")]
    public int goblinsPerChunk = 50;           // Number of minions per "chunk"
    public float spawnInterval = 0.5f;         // Time between spawns
    public float minionHealthMultiplierIncrement = 0.2f; // +20% HP per 50 goblins
    public float minionSpeedIncrement = 0.5f;  // +0.5 move speed per 50 goblins
    public float bossHealthIncrement = 2000f;  // Boss HP increase per spawn
    public float bossSpeedIncrement = 1f;      // Boss speed increase per spawn

    [Header("UI Settings")]
    public WaveCounterUI waveCounterUI;
    public EnemiesRemainingUI enemiesRemainingUI;

    private int totalGoblinsSpawned = 0;
    private float currentMinionHealthMultiplier = 1f;
    private float currentMinionSpeedMultiplier = 1f;
    private float currentBossHealth = 0f;
    private float currentBossSpeed = 0f;
    public bool endlessActive = false;

    public void StartEndless()
    {
        if (spawner == null)
        {
            Debug.LogError("Spawner not assigned in EndlessModeManager!");
            return;
        }

        endlessActive = true;
        totalGoblinsSpawned = 0;
        currentMinionHealthMultiplier = 1f;
        currentMinionSpeedMultiplier = 1f;
        currentBossHealth = spawner.bossPrefab.GetComponent<BossStats>().maxHealth;
        currentBossSpeed = spawner.bossPrefab.GetComponent<EnemyMovement>().baseSpeed;

        // Update UI for unlimited mode
        if (waveCounterUI != null)
            waveCounterUI.SetUnlimitedMode();
        if (enemiesRemainingUI != null)
            enemiesRemainingUI.SetUnlimitedMode();

        StartCoroutine(SpawnEndlessRoutine());
    }

    private IEnumerator SpawnEndlessRoutine()
    {
        while (endlessActive)
        {
            int goblinsThisChunk = goblinsPerChunk;

            for (int i = 0; i < goblinsThisChunk; i++)
            {
                GameObject goblin = Instantiate(spawner.enemyPrefab, spawner.spawnPoint.position, Quaternion.identity);

                // Assign stats
                MinionStats stats = goblin.GetComponent<MinionStats>();
                if (stats != null)
                {
                    stats.Initialise(1);
                    stats.maxHealth *= currentMinionHealthMultiplier;
                    stats.currentHealth = stats.maxHealth;

                    EnemyMovement movement = goblin.GetComponent<EnemyMovement>();
                    if (movement != null)
                    {
                        movement.waypoints = spawner.GetWaypoints();
                        movement.baseSpeed *= currentMinionSpeedMultiplier;
                    }
                }

                EnemyDeathNotifier notifier = goblin.GetComponent<EnemyDeathNotifier>();
                if (notifier == null)
                    notifier = goblin.AddComponent<EnemyDeathNotifier>();
                notifier.Initialize(spawner);

                totalGoblinsSpawned++;

                // Every 50 goblins, increase minion HP and speed
                if (totalGoblinsSpawned % 50 == 0)
                {
                    currentMinionHealthMultiplier += minionHealthMultiplierIncrement;
                    currentMinionSpeedMultiplier += minionSpeedIncrement;
                }

                // Every 100 goblins, spawn a boss
                if (totalGoblinsSpawned % 100 == 0)
                {
                    GameObject boss = Instantiate(spawner.bossPrefab, spawner.spawnPoint.position, Quaternion.identity);
                    BossStats bossStats = boss.GetComponent<BossStats>();
                    EnemyMovement bossMove = boss.GetComponent<EnemyMovement>();

                    if (bossStats != null)
                    {
                        bossStats.maxHealth = currentBossHealth + bossHealthIncrement;
                        bossStats.currentHealth = bossStats.maxHealth;
                        currentBossHealth = bossStats.maxHealth; // Update for next boss
                    }

                    if (bossMove != null)
                    {
                        bossMove.waypoints = spawner.GetWaypoints();
                        bossMove.baseSpeed = currentBossSpeed + bossSpeedIncrement;
                        currentBossSpeed = bossMove.baseSpeed; // Update for next boss
                    }

                    EnemyDeathNotifier bossNotifier = boss.GetComponent<EnemyDeathNotifier>();
                    if (bossNotifier == null)
                        bossNotifier = boss.AddComponent<EnemyDeathNotifier>();
                    bossNotifier.Initialize(spawner);
                }

                yield return new WaitForSeconds(spawnInterval);
            }

            // Wait until all enemies are dead before next chunk
            while (spawner.enemiesAlive > 0)
                yield return null;
        }
    }
}
