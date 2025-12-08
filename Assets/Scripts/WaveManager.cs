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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip waveStartClip;
    public AudioClip midWaveClip;

    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveStarted;
    public event WaveEvent OnWaveEnded;

    private bool isSpawning = false;
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
        if (isSpawning || currentWave >= totalWaves)
            return;

        currentWave++;
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        isSpawning = true;

        // Fade out the Start Wave button
        if (startWaveButton != null)
            yield return StartCoroutine(FadeButton(0f));

        // Play wave start horn
        if (audioSource != null && waveStartClip != null)
            audioSource.PlayOneShot(waveStartClip);

        // Update spawner for this wave
        spawner.currentRound = currentWave;
        spawner.totalEnemies = 10 + (10 * currentWave); // scale enemies per wave

        OnWaveStarted?.Invoke(currentWave);

        // Spawn enemies
        yield return StartCoroutine(spawner.SpawnEnemiesRoutine());

        // Wait until all enemies are dead
        while (spawner.enemiesAlive > 0)
            yield return null;

        OnWaveEnded?.Invoke(currentWave);

        // Wait before enabling button
        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWave < totalWaves && startWaveButton != null)
            yield return StartCoroutine(FadeButton(1f));

        isSpawning = false;
    }

    private IEnumerator FadeButton(float targetAlpha)
    {
        if (buttonCanvasGroup == null) yield break;

        float startAlpha = buttonCanvasGroup.alpha;
        float elapsed = 0f;

        buttonCanvasGroup.interactable = targetAlpha > 0;
        buttonCanvasGroup.blocksRaycasts = targetAlpha > 0;

        while (elapsed < 0.5f)
        {
            elapsed += Time.deltaTime;
            buttonCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / 0.5f);
            yield return null;
        }

        buttonCanvasGroup.alpha = targetAlpha;
        buttonCanvasGroup.interactable = targetAlpha > 0;
        buttonCanvasGroup.blocksRaycasts = targetAlpha > 0;
    }
}
