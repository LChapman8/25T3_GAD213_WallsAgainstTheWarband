using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public EnemySpawner spawner;
    public int currentWave = 0;
    public int totalWaves = 5;
    public float timeBetweenWaves = 5f;
    public Button startWaveButton;         // Assign your Start Wave button in Inspector
    public float fadeDuration = 0.5f;      // Duration for fade in/out
    public float pulseAmount = 0.5f;       // How much to fade during pulse
    public float pulseDuration = 0.3f;     // How long the pulse takes

    [Header("Audio")]
    public AudioSource audioSource;        // Main audio source
    public AudioClip waveStartClip;        // First announcer clip (war horn)
    public AudioClip midWaveClip;          // Second announcer clip (halfway through)

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

        // Fade out Start Wave button
        if (startWaveButton != null)
            yield return StartCoroutine(FadeButton(0f));

        // Play first announcer clip
        if (audioSource != null && waveStartClip != null)
        {
            audioSource.PlayOneShot(waveStartClip);

            // Play mid-wave clip halfway through first clip
            if (midWaveClip != null)
                StartCoroutine(PlayMidWaveClipAfterDelay(waveStartClip.length / 2f));
        }

        OnWaveStarted?.Invoke(currentWave);

        spawner.totalEnemies += 2 * currentWave;

        // Spawn enemies
        yield return StartCoroutine(spawner.SpawnEnemiesRoutine());

        while (spawner.enemiesAlive > 0)
        {
            yield return null;
        }

        OnWaveEnded?.Invoke(currentWave);

        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWave < totalWaves && startWaveButton != null)
            yield return StartCoroutine(FadeButton(1f));

        isSpawning = false;

        if (currentWave >= totalWaves)
            Debug.Log("All waves complete!");
    }

    private IEnumerator PlayMidWaveClipAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null && midWaveClip != null)
            audioSource.PlayOneShot(midWaveClip);

        // Pulse the button if it exists
        if (buttonCanvasGroup != null)
            yield return StartCoroutine(PulseButton());
    }

    private IEnumerator FadeButton(float targetAlpha)
    {
        if (buttonCanvasGroup == null)
            yield break;

        float startAlpha = buttonCanvasGroup.alpha;
        float elapsed = 0f;

        buttonCanvasGroup.interactable = targetAlpha > 0f;
        buttonCanvasGroup.blocksRaycasts = targetAlpha > 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            buttonCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        buttonCanvasGroup.alpha = targetAlpha;
        buttonCanvasGroup.interactable = targetAlpha > 0f;
        buttonCanvasGroup.blocksRaycasts = targetAlpha > 0f;
    }

    private IEnumerator PulseButton()
    {
        if (buttonCanvasGroup == null)
            yield break;

        float originalAlpha = buttonCanvasGroup.alpha;
        float targetAlpha = Mathf.Clamp01(originalAlpha - pulseAmount);
        float elapsed = 0f;

        // Fade down
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            buttonCanvasGroup.alpha = Mathf.Lerp(originalAlpha, targetAlpha, elapsed / pulseDuration);
            yield return null;
        }

        // Fade back up
        elapsed = 0f;
        while (elapsed < pulseDuration)
        {
            elapsed += Time.deltaTime;
            buttonCanvasGroup.alpha = Mathf.Lerp(targetAlpha, originalAlpha, elapsed / pulseDuration);
            yield return null;
        }

        buttonCanvasGroup.alpha = originalAlpha;
    }
}
