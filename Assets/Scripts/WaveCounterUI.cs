using UnityEngine;
using TMPro;

public class WaveCounterUI : MonoBehaviour
{
    public WaveManager waveManager; 
    public TextMeshProUGUI waveText; 

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
