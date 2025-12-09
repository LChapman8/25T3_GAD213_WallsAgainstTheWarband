using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner spawner;
    public int currentWave = 0;
    public int totalWaves = 5;
    public float timeBetweenWaves = 5f;
    public Button startWaveButton;

    [Header("Rewards")]
    public int goldPerWave = 100;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip waveStartClip;

    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveStarted;

    private bool isSpawning = false;
    private bool bossWaveStarted = false;
    private CanvasGroup buttonCanvasGroup;

    private void Awake()
    {
        if (startWaveButton != null)
        {
            buttonCanvasGroup = startWaveButton.GetComponent<CanvasGroup>();
            if (buttonCanvasGroup == null)
                buttonCanvasGroup = startWaveButton.gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void StartNextWave()
    {
        if (isSpawning) return;

        currentWave++;

        if (currentWave <= totalWaves)
            StartCoroutine(WaveRoutine());
        else if (!bossWaveStarted)
            StartCoroutine(BossWaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        isSpawning = true;

        FadeOutButton();
        audioSource?.PlayOneShot(waveStartClip);

        spawner.currentRound = currentWave;
        spawner.totalEnemies = 10 + (10 * currentWave);

        OnWaveStarted?.Invoke(currentWave);

        yield return StartCoroutine(spawner.SpawnEnemiesRoutine());

        // wait until wave is fully cleared
        while (spawner.enemiesAlive > 0)
            yield return null;

        //  END OF WAVE GOLD REWARD
        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldPerWave);

        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWave < totalWaves)
            FadeInButton();
        else
            StartCoroutine(BossWaveRoutine());

        isSpawning = false;
    }

    private IEnumerator BossWaveRoutine()
    {
        bossWaveStarted = true;
        isSpawning = true;

        if (startWaveButton != null)
            startWaveButton.gameObject.SetActive(false);

        audioSource?.PlayOneShot(waveStartClip);

        // Inform UI that this is a boss round
        OnWaveStarted?.Invoke(-1); // -1 = Boss Round

        yield return StartCoroutine(spawner.SpawnBossRoutine());

        while (spawner.enemiesAlive > 0)
            yield return null;

        Debug.Log("BOSS DEFEATED – YOU WIN");
        isSpawning = false;
    }

    void FadeOutButton()
    {
        if (buttonCanvasGroup == null) return;
        buttonCanvasGroup.alpha = 0f;
        buttonCanvasGroup.interactable = false;
        buttonCanvasGroup.blocksRaycasts = false;
    }

    void FadeInButton()
    {
        if (buttonCanvasGroup == null) return;
        buttonCanvasGroup.alpha = 1f;
        buttonCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
    }
}
