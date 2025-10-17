using UnityEngine;
using TMPro;

public class WaveCounterUI : MonoBehaviour
{
    public WaveManager waveManager; // Drag your WaveManager here
    public TextMeshProUGUI waveText; // Drag your TMP text component here

    private void OnEnable()
    {
        if (waveManager != null)
            waveManager.OnWaveStarted += UpdateWaveUI;
    }

    private void OnDisable()
    {
        if (waveManager != null)
            waveManager.OnWaveStarted -= UpdateWaveUI;
    }

    private void UpdateWaveUI(int waveNumber)
    {
        if (waveText != null)
            waveText.text = $"Wave: {waveNumber}/{waveManager.totalWaves}";
    }
}
